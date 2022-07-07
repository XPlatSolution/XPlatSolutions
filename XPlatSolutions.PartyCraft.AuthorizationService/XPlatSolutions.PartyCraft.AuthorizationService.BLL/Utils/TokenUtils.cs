using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Utils;

public class TokenUtils : ITokenUtils
{
    private readonly IOptions<AppOptions> _appOptions;
    private readonly IUsersAccess _usersAccess;
    private readonly ITokensAccess _tokensAccess;

    public TokenUtils(IOptions<AppOptions> appOptions, IUsersAccess usersAccess, ITokensAccess tokensAccess)
    {
        _appOptions = appOptions;
        _usersAccess = usersAccess;
        _tokensAccess = tokensAccess;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appOptions.Value.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateIdToken(string id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appOptions.Value.IdSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("ident", id) }),
            Expires = DateTime.UtcNow.AddHours(_appOptions.Value.ResetPasswordInHoursTTL),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateIdToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appOptions.Value.IdSecret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            var value = jwtToken?.Claims.First(x => x.Type == "ident").Value;
            return value;
        }
        catch
        {
            return null;
        }
    }

    public string? ValidateToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appOptions.Value.Secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            var value = jwtToken?.Claims.First(x => x.Type == "id").Value;
            return value;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RefreshToken> GenerateRefreshToken(string ipAddress, string userId)
    {
        var refreshToken = new RefreshToken
        {
            Value = await GetUniqueToken(),
            ExpireDateTime = DateTime.UtcNow.AddDays(7),
            CreationDateTime = DateTime.UtcNow,
            CreatorIp = ipAddress,
            UserId = userId
        };

        return  refreshToken;
    }

    public Task AddToken(RefreshToken token)
    {
        return _tokensAccess.AddToken(token);
    }

    public Task RemoveAllOldTokens(string userId)
    {
        return _tokensAccess.RemoveAllOldTokens(userId);
    }

    public async Task<string> GetUserIdByRefreshToken(string refreshToken)
    {
        return (await _tokensAccess.GetTokenByValue(refreshToken)).UserId;
    }

    public Task<RefreshToken> GetTokenByValue(string refreshToken)
    {
        return _tokensAccess.GetTokenByValue(refreshToken);
    }

    public Task<List<RefreshToken>> GetTokensByUserId(string userId)
    {
        return _tokensAccess.GetTokensByUserId(userId);
    }

    public Task UpdateToken(string tokenId, RefreshToken token)
    {
        return _tokensAccess.UpdateToken(tokenId, token);
    }

    private async Task<string> GetUniqueToken()
    {
        while (true)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var containsToken = await _tokensAccess.Contains(token);

            if (containsToken) continue;

            return token;
        }
    }
}