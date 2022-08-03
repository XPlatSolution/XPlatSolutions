using System.Reflection;
using System.Text.RegularExpressions;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Services;

public class UserService : IUserService
{
    private readonly IUsersAccess _usersAccess;
    private readonly IOptions<AppOptions> _appOptions;
    private readonly ITokenUtils _tokenUtils;
    private readonly IActivationCodeAccess _activationCodeAccess;
    private readonly IQueueWriter _queueWriter;
    private readonly IPasswordChangeRequestAccess _passwordChangeRequestAccess;
    private readonly IResponseFactory _responseFactory;
    private readonly IOperationResultFactory _operationResultFactory;

    private static readonly string ResetHtml =
        ReadResource("XPlatSolutions.PartyCraft.AuthorizationService.BLL.Resources.reset.html");

    private static readonly string VerifyHtml =
        ReadResource("XPlatSolutions.PartyCraft.AuthorizationService.BLL.Resources.verify.html");

    private static string ReadResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) return "";

        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();

        return result;
    }

    public UserService(IUsersAccess usersAccess, IOptions<AppOptions> appOptions, ITokenUtils tokenUtils,
        IActivationCodeAccess activationCodeAccess, IQueueWriter queueWriter, IPasswordChangeRequestAccess passwordChangeRequestAccess,
        IResponseFactory responseFactory, IOperationResultFactory operationResultFactory)
    {
        _usersAccess = usersAccess;
        _appOptions = appOptions;
        _tokenUtils = tokenUtils;
        _activationCodeAccess = activationCodeAccess;
        _queueWriter = queueWriter;
        _passwordChangeRequestAccess = passwordChangeRequestAccess;
        _responseFactory = responseFactory;
        _operationResultFactory = operationResultFactory;
    }

    public async Task<OperationResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request, string userIp)
    {
        var user = await _usersAccess.GetUser(request.Email, request.Password);

        var token = _tokenUtils.GenerateToken(user);
        var refreshToken = await _tokenUtils.GenerateRefreshToken(userIp, user.Id);

        if (_appOptions.Value.RemoveOldAndRevokedTokens)
            await _tokenUtils.RemoveAllOldTokens(user.Id);

        await _tokenUtils.AddToken(refreshToken);

        return _operationResultFactory.CreateOperationResult(
            _responseFactory.CreateAuthenticateResponse(user, token, refreshToken.Value)
            );
    }

    public async Task<OperationResult<AuthenticateResponse>> RefreshToken(string refreshToken, string userIp)
    {
        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        var userTokens = await _tokenUtils.GetTokensByUserId(token.UserId);
        var user = await _usersAccess.GetUserById(token.UserId);

        if (token.IsRevoked)
        {
            await RevokeAllChildRefreshToken(token, userTokens, $"Attempted reuse of revoked ancestor token: {token.Value}", userIp);

            return _operationResultFactory.CreateOperationResult<AuthenticateResponse>(
                null, StatusCode.HandledError, "Attempted reuse of revoked ancestor token"
            );
        }

        var newToken = await RevokeAndRefreshToken(token, userIp);

        if (_appOptions.Value.RemoveOldAndRevokedTokens)
            await _tokenUtils.RemoveAllOldTokens(newToken.UserId);

        await _tokenUtils.AddToken(newToken);

        var accessToken = _tokenUtils.GenerateToken(user);

        return _operationResultFactory.CreateOperationResult(
            _responseFactory.CreateAuthenticateResponse(user, accessToken, newToken.Value)
        );
    }

    public async Task<OperationResult> ResendVerificationCode(User? user)
    {
        await SendActivationCode(user);
        return _operationResultFactory.CreateOperationResult();
    }

    public async Task<OperationResult> Verify(string verifyCode)
    {
        var code = await _activationCodeAccess.GetActivationCode(verifyCode);

        await _usersAccess.SetVerified(code.UserId);
        await _activationCodeAccess.RemoveAllActivationCodes(code.UserId);

        return _operationResultFactory.CreateOperationResult();
    }

    public async Task<OperationResult<RestorePasswordResponse>> RestorePasswordRequest(RestorePasswordRequest? request)
    {
        var user = await _usersAccess.GetUserByEmail(request.Email);

        var passwordChangeModel = new PasswordChangeRequest { RequestDateTime = DateTime.UtcNow, UserId = user.Id };
        await _passwordChangeRequestAccess.AddPasswordChangeRequests(passwordChangeModel);

        var token = _tokenUtils.GenerateIdToken(passwordChangeModel.Id);
        var resetUrl = $@"https://georgespring.com/Profile/ResetPassword?token={token}";

        var resetPasswordMessage = GenerateResetPasswordMessage(user, resetUrl);

        _queueWriter.WriteEmailMessageTask(request.Email, resetPasswordMessage, "Your password reset link");

        return _operationResultFactory.CreateOperationResult(
            _responseFactory.CreateRestorePasswordResponse(true));
    }

    public async Task<OperationResult<RestorePasswordResponse>> RestorePassword(ResetPasswordRequest? request)
    {
        var id = _tokenUtils.ValidateIdToken(request.Token);

        var passwordChangeRequest = await _passwordChangeRequestAccess.GetPasswordChangeRequest(id);

        if (passwordChangeRequest.RequestDateTime.AddHours(_appOptions.Value.ResetPasswordInHoursTTL) < DateTime.UtcNow)
            throw new XPlatSolutionsException("Request expired");

        await _usersAccess.ChangePassword(passwordChangeRequest.UserId, BCrypt.Net.BCrypt.HashPassword(request.NewPassword));

        return _operationResultFactory.CreateOperationResult(
            _responseFactory.CreateRestorePasswordResponse(true));
    }

    private string GenerateResetPasswordMessage(User user, string resetUrl)
    {
        return ResetHtml.Replace("@@name@@", $"{user.LastName} {user.Name}").Replace("@@btn@@", resetUrl).Replace("@@hours@@", _appOptions.Value.ResetPasswordInHoursTTL.ToString());
    }

    public async Task<OperationResult<RegisterResponse>> Register(RegisterRequest? request)
    {
        var userModel = MapRegisterRequestToUser(request);

        var isCreated = await _usersAccess.AddUser(userModel);

        if (!isCreated)
            throw new XPlatSolutionsException("Cannot create this user");

        await SendActivationCode(userModel);

        return _operationResultFactory.CreateOperationResult(
            _responseFactory.CreateRegisterResponse(true));
    }

    private async Task SendActivationCode(User userModel)
    {
        var activationCode = Guid.NewGuid().ToString();

        var codes = await _activationCodeAccess.GetActivationCodes(userModel.Id);

        if (codes != null && codes.Any())
        {
            var maxRequest = codes.Max(x => x.CreationDateTime);

            if (maxRequest.AddMinutes(1) > DateTime.UtcNow)
                throw new XPlatSolutionsException("Too many requests, please wait 1 minute");
        }

        while (await _activationCodeAccess.GetActivationCode(activationCode) != null)
        {
            activationCode = Guid.NewGuid().ToString();
        }

        await _activationCodeAccess.AddActivationCode(new ActivationCode
        {
            CreationDateTime = DateTime.UtcNow,
            Value = activationCode,
            UserId = userModel.Id
        });

        _queueWriter.WriteEmailMessageTask(userModel.Email, GenerateEmailActivationMessage($"https://georgespring.com/Verify/{activationCode}", $"{userModel.LastName} {userModel.Name}"), "Your activation link");
    }

    private static string GenerateEmailActivationMessage(string verificationUrl, string login)
    {
        return VerifyHtml.Replace("@@name@@", login).Replace("@@btn@@", verificationUrl);
    }

    public async Task<OperationResult> RevokeToken(string refreshToken, string userIp)
    {
        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        await RevokeRefreshToken(token, userIp, "Revoked without replacement");

        return _operationResultFactory.CreateOperationResult();
    }

    public async Task<OperationResult<User?>> GetById(string userId)
    {
#pragma warning disable CS8619
        return _operationResultFactory.CreateOperationResult(await _usersAccess.GetUserById(userId));
#pragma warning restore CS8619
    }

    public async Task<OperationResult<List<User>>> GetAll()
    {
        return _operationResultFactory.CreateOperationResult(await _usersAccess.GetAll());
    }

    private async Task RevokeAllChildRefreshToken(RefreshToken refreshToken, List<RefreshToken> userTokens, string reason, string ipAddress)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;

        var childToken = userTokens.SingleOrDefault(x => x.Value == refreshToken.ReplacedByToken);

        if (childToken == null) return;

        if (childToken.IsActive)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeAllChildRefreshToken(childToken, userTokens, ipAddress, reason);
    }

    private async Task RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = "", string replacedByToken = "")
    {
        token.RevokeDateTime = DateTime.UtcNow;
        token.RevokerIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;

        await _tokenUtils.UpdateToken(token.Id, token);
    }

    private async Task<RefreshToken> RevokeAndRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = await _tokenUtils.GenerateRefreshToken(ipAddress, refreshToken.UserId);
        await RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Value);
        return newRefreshToken;
    }

    private static User MapRegisterRequestToUser(RegisterRequest request)
    {
        return new User
        {
            Email = request.Email,
            LastName = request.LastName,
            Name = request.Name,
            Phone = request.Phone,
            SecondName = request.SecondName,
            BirthdayDateTime = request.BirthdayDateTime,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmailVerified = false,
        };
    }
}