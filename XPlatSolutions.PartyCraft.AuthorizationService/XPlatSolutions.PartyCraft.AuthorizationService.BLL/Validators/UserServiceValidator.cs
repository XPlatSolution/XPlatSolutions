using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Validators;

public class UserServiceValidator : IUserService
{
    private readonly IUsersAccess _usersAccess;
    private readonly IOptions<AppOptions> _appOptions;
    private readonly ITokenUtils _tokenUtils;
    private readonly IActivationCodeAccess _activationCodeAccess;
    private readonly IOperationResultFactory _operationResultFactory;

    private readonly IUserService _userService;

    public UserServiceValidator(IUserService userService, IUsersAccess usersAccess, IOptions<AppOptions> appOptions, ITokenUtils tokenUtils,
    IActivationCodeAccess activationCodeAccess, IOperationResultFactory operationResultFactory)
    {
        _userService = userService;
        _usersAccess = usersAccess;
        _appOptions = appOptions;
        _tokenUtils = tokenUtils;
        _activationCodeAccess = activationCodeAccess;
        _operationResultFactory = operationResultFactory;
    }

    public async Task<OperationResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request, string userIp)
    {
        var user = await _usersAccess.GetUser(request.Email, request.Password);

        if (user == null)
            return _operationResultFactory.CreateOperationResult<AuthenticateResponse>(null, StatusCode.HandledError,
                "Email or password is incorrect");

        return await _userService.Authenticate(request, userIp);
    }

    public async Task<OperationResult<AuthenticateResponse>> RefreshToken(string refreshToken, string userIp)
    {
        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        if (token == null) 
            return _operationResultFactory.CreateOperationResult<AuthenticateResponse>(null, StatusCode.HandledError,
                "Invalid token");

        if (!token.IsActive)
            return _operationResultFactory.CreateOperationResult<AuthenticateResponse>(null, StatusCode.HandledError,
                "Token is not active");

        var user = await _usersAccess.GetUserById(token.UserId);

        if (user == null)
            return _operationResultFactory.CreateOperationResult<AuthenticateResponse>(null, StatusCode.HandledError,
                "User does not exist");

        return await _userService.RefreshToken(refreshToken, userIp);
    }

    public async Task<OperationResult<RegisterResponse>> Register(RegisterRequest? request)
    {
        if (request == null)
            return _operationResultFactory.CreateOperationResult<RegisterResponse>(null, StatusCode.HandledError,
                "User does not exist");

        if (!ValidateEmail(request.Email, out var emailError))
            return _operationResultFactory.CreateOperationResult<RegisterResponse>(null, StatusCode.HandledError,
                emailError);

        if (!ValidatePassword(request.Password, out var passwordError))
            return _operationResultFactory.CreateOperationResult<RegisterResponse>(null, StatusCode.HandledError,
                passwordError);

        if (await _usersAccess.GetUserByEmail(request.Email) != null)
            return _operationResultFactory.CreateOperationResult<RegisterResponse>(null, StatusCode.HandledError,
                "Email \"" + request.Email + "\" is already taken");

        return await _userService.Register(request);
    }

    public async Task<OperationResult> RevokeToken(string refreshToken, string userIp)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return _operationResultFactory.CreateOperationResult(StatusCode.HandledError, "Token is required");

        var token = await _tokenUtils.GetTokenByValue(refreshToken);

        if (token == null || !token.IsActive)
            return _operationResultFactory.CreateOperationResult(StatusCode.HandledError, "Invalid token");

        return await _userService.RevokeToken(refreshToken, userIp);
    }

    public Task<OperationResult<User?>> GetById(string userId)
    {
        return _userService.GetById(userId);
    }

    public Task<OperationResult<List<User>>> GetAll()
    {
        return _userService.GetAll();
    }

    public async Task<OperationResult> Verify(string verifyCode)
    {
        var code = await _activationCodeAccess.GetActivationCode(verifyCode);

        if (code.CreationDateTime.AddMinutes(_appOptions.Value.ActivationCodeMinutesTTL) < DateTime.UtcNow)
            return _operationResultFactory.CreateOperationResult(StatusCode.HandledError, "TTL lower than now");

        return await _userService.Verify(verifyCode);
    }

    public async Task<OperationResult<RestorePasswordResponse>> RestorePasswordRequest(RestorePasswordRequest? request)
    {
        if (request == null)
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "Payload is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "Email is required");

        if (!ValidateEmail(request.Email, out var errorMessage))
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                errorMessage);

        var user = await _usersAccess.GetUserByEmail(request.Email);

        if (user == null)
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "User with this email does not exist");

        return await _userService.RestorePasswordRequest(request);
    }

    public async Task<OperationResult<RestorePasswordResponse>> RestorePassword(ResetPasswordRequest? request)
    {
        if (request?.Token == null)
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "Token is required");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "New password is required");

        if (!ValidatePassword(request.NewPassword, out var errorMessage))
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                errorMessage);

        var id = _tokenUtils.ValidateIdToken(request.Token);

        if (id == null)
            return _operationResultFactory.CreateOperationResult<RestorePasswordResponse>(null, StatusCode.HandledError,
                "Invalid token");

        return await _userService.RestorePassword(request);
    }

    public Task<OperationResult> ResendVerificationCode(User user)
    {
        return _userService.ResendVerificationCode(user);
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
}