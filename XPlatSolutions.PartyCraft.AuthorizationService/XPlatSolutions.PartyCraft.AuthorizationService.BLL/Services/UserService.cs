using System.Text.RegularExpressions;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
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

    public UserService(IUsersAccess usersAccess, IOptions<AppOptions> appOptions, ITokenUtils tokenUtils,
        IActivationCodeAccess activationCodeAccess, IQueueWriter queueWriter, IPasswordChangeRequestAccess passwordChangeRequestAccess)
    {
        _usersAccess = usersAccess;
        _appOptions = appOptions;
        _tokenUtils = tokenUtils;
        _activationCodeAccess = activationCodeAccess;
        _queueWriter = queueWriter;
        _passwordChangeRequestAccess = passwordChangeRequestAccess;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, string userIp)
    {
        var user = await _usersAccess.GetUser(request.Login, request.Password);

        if (user == null)
            throw new AuthenticateException("Login or password is incorrect");

        var token = _tokenUtils.GenerateToken(user);
        var refreshToken = await _tokenUtils.GenerateRefreshToken(userIp, user.Id);

        if (_appOptions.Value.RemoveOldAndRevokedTokens)
            await _tokenUtils.RemoveAllOldTokens(user.Id);

        await _tokenUtils.AddToken(refreshToken);

        return new AuthenticateResponse(user, token, refreshToken.Value);
    }

    public async Task<AuthenticateResponse> RefreshToken(string refreshToken, string userIp)
    {
        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        if (token == null)
            throw new AuthenticateException("Invalid token");

        var userTokens = await _tokenUtils.GetTokensByUserId(token.UserId);
        var user = await _usersAccess.GetUserById(token.UserId);

        if(user == null)
            throw new AuthenticateException("User does not exist");

        if (token.IsRevoked)
        {
            await RevokeAllChildRefreshToken(token, userTokens, $"Attempted reuse of revoked ancestor token: {token.Value}", userIp);
            throw new AuthenticateException($"Attempted reuse of revoked ancestor token");
        }

        if (!token.IsActive)
            throw new AuthenticateException("Token is not active");

        var newToken = await RevokeAndRefreshToken(token, userIp);

        if (_appOptions.Value.RemoveOldAndRevokedTokens)
            await _tokenUtils.RemoveAllOldTokens(newToken.UserId);

        await _tokenUtils.AddToken(newToken);

        var accessToken = _tokenUtils.GenerateToken(user);

        return new AuthenticateResponse(user, accessToken, newToken.Value);
    }

    public async Task ResendVerificationCode(User user)
    {
        await SendActivationCode(user);
    }

    public async Task<bool> Verify(string verifyCode)
    {
        var code = await _activationCodeAccess.GetActivationCode(verifyCode);

        if (code.CreationDateTime.AddMinutes(_appOptions.Value.ActivationCodeMinutesTTL) < DateTime.UtcNow)
            return false;

        await _usersAccess.SetVerified(code.UserId);
        await _activationCodeAccess.RemoveAllActivationCodes(code.UserId);

        return true;
    }

    public async Task<RestorePasswordResponse> RestorePasswordRequest(RestorePasswordRequest? request)
    {
        if (request == null)
            throw new XPlatSolutionsException("Payload is required");

        if (request.Email == null)
            throw new XPlatSolutionsException("Email is required");

        if (!ValidateEmail(request.Email, out var errorMessage))
        {
            throw new XPlatSolutionsException(errorMessage);
        }

        var user = await _usersAccess.GetUserByEmail(request.Email);

        if (user == null)
            throw new XPlatSolutionsException("User with this email does not exist");

        var passwordChangeModel = new PasswordChangeRequest { RequestDateTime = DateTime.UtcNow, UserId = user.Id };
        await _passwordChangeRequestAccess.AddPasswordChangeRequests(passwordChangeModel);

        var token = _tokenUtils.GenerateIdToken(passwordChangeModel.Id);
        var resetUrl = $@"https://georgespring.com/Profile/ResetPassword?token={token}";

        var resetPasswordMessage = GenerateResetPasswordMessage(user, resetUrl);

        await _queueWriter.WriteEmailMessageTask(request.Email, resetPasswordMessage);

        return new RestorePasswordResponse { Success = true };
    }

    public async Task<RestorePasswordResponse> RestorePassword(ResetPasswordRequest? request)
    {
        if (request?.Token == null)
            throw new XPlatSolutionsException("Token is required");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new XPlatSolutionsException("New password is required");

        if (!ValidatePassword(request.NewPassword, out var errorMessage))
            throw new XPlatSolutionsException(errorMessage);

        var id = _tokenUtils.ValidateIdToken(request.Token);

        if (id == null)
            throw new XPlatSolutionsException("Invalid token");

        var passwordChangeRequest = await _passwordChangeRequestAccess.GetPasswordChangeRequest(id);

        if (passwordChangeRequest.RequestDateTime.AddHours(_appOptions.Value.ResetPasswordInHoursTTL) < DateTime.UtcNow)
            throw new XPlatSolutionsException("Request expired");

        await _usersAccess.ChangePassword(passwordChangeRequest.UserId, BCrypt.Net.BCrypt.HashPassword(request.NewPassword));

        return new RestorePasswordResponse { Success = true };
    }

    private string GenerateResetPasswordMessage(User user, string resetUrl)
    {
        return $"<br/><br/>Hi {user.LastName} {user.Name}! We're sending you this email because you" +
               " requested a password reset. Click on this link to create a new password:" +
               " <br/><br/><a href='" + resetUrl + "'>" + "Click me" + "</a><br/><br/>" +
               $" This link will expire in {_appOptions.Value.ResetPasswordInHoursTTL} hours. After that, you'll need " +
               "to submit a new request to reset your password." +
               " If you didn't request a password reset, you can ignore this email. Your password will not be changed.";
    }

    public async Task<RegisterResponse> Register(RegisterRequest? request)
    {
        if (request == null)
            throw new XPlatSolutionsException("Payload is required");

        //if (!ValidateLogin(request.Email, out var loginError))
        //    throw new XPlatSolutionsException(loginError);

        if (!ValidateEmail(request.Email, out var emailError))
            throw new XPlatSolutionsException(emailError);

        if (!ValidatePassword(request.Password, out var passwordError))
            throw new XPlatSolutionsException(passwordError);
        
        if (await _usersAccess.GetUserByEmail(request.Email) != null)
            throw new XPlatSolutionsException("Email \"" + request.Email + "\" is already taken");

        var userModel = MapRegisterRequestToUser(request);
        var isCreated = await _usersAccess.AddUser(userModel);

        if (!isCreated)
            throw new XPlatSolutionsException("Cannot create this user");

        await SendActivationCode(userModel);

        return new RegisterResponse { Success = true };
    }

    private async Task SendActivationCode(User userModel)
    {
        var activationCode = Guid.NewGuid().ToString();

        var codes = await _activationCodeAccess.GetActivationCodes(userModel.Id);

        if (codes != null && codes.Any())
        {
            var maxRequest = codes.Max(x => x.CreationDateTime);

            if (maxRequest.AddMinutes(1) < DateTime.UtcNow)
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

        await _queueWriter.WriteEmailMessageTask(userModel.Email, GenerateEmailActivationMessage("", $"{userModel.LastName} {userModel.Name}"));
    }

    private static string GenerateEmailActivationMessage(string verificationUrl, string login)
    {
        return $"<br/><br/>Hi {login}! We are excited to tell you that your account is" +
               " successfully created. Please click on the below link to verify your account" +
               " <br/><br/><a href='" + verificationUrl + "'>" + verificationUrl + "</a> ";
    }

    private static bool ValidateLogin(string input, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new Exception("Login should not be empty");
        }

        var isValid = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{3,12}$");
        
        if (isValid.IsMatch(input)) return true;
        errorMessage = "Login must contain from 3 to 12 characters and consist only of numbers and letters";
        return false;
    }

    private static bool ValidateEmail(string input, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new Exception("Email should not be empty");
        }

        var isValid = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");


        if (isValid.IsMatch(input)) return true;
        errorMessage = "Invalid Email";
        return false;

    }

    private static bool ValidatePassword(string input, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new Exception("Password should not be empty");
        }

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMiniMaxChars = new Regex(@".{8,20}");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        if (!hasLowerChar.IsMatch(input))
        {
            errorMessage = "Password should contain at least one lower case letter";
            return false;
        }

        if (!hasUpperChar.IsMatch(input))
        {
            errorMessage = "Password should contain at least one upper case letter";
            return false;
        }
        if (!hasMiniMaxChars.IsMatch(input))
        {
            errorMessage = "Password should not be less than 8 or greater than 12 characters";
            return false;
        }
        if (!hasNumber.IsMatch(input))
        {
            errorMessage = "Password should contain at least one numeric value";
            return false;
        }

        if (hasSymbols.IsMatch(input)) return true;
        errorMessage = "Password should contain at least one special case characters";
        return false;
    }

    public async Task RevokeToken(string refreshToken, string userIp)
    {
        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        if (token == null || !token.IsActive)
            throw new AuthenticateException("Invalid token");

        await RevokeRefreshToken(token, userIp, "Revoked without replacement");
    }

    public async Task<User?> GetById(string userId)
    {
        return await _usersAccess.GetUserById(userId);
    }

    public Task<List<User>> GetAll()
    {
        return _usersAccess.GetAll();
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmailVerified = false,
        };
    }
}