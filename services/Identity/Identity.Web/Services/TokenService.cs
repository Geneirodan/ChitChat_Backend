using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Persistence;
using Identity.Web.Options;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Web.Jwt;
using static System.StringComparison;
using static Microsoft.IdentityModel.Tokens.SecurityAlgorithms;

namespace Identity.Web.Services;

internal sealed class TokenService(
    IOptions<JwtOptions> jwtOptions,
    IOptions<ExpirationOptions> expirationOptions,
    ApplicationContext context,
    UserManager<User> userManager,
    TimeProvider timeProvider
) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ExpirationOptions _expirationOptions = expirationOptions.Value;
    private readonly SymmetricSecurityKey _symmetricSecurityKey = new(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));

    public async Task<AccessTokenResponse?> GenerateTokens(User user)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        ];
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_expirationOptions.AccessToken)),
            signingCredentials: new SigningCredentials(_symmetricSecurityKey, HmacSha256));

        var refreshToken = GenerateRefreshToken();

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Value = refreshToken,
            ExpiresAt = timeProvider.GetUtcNow().AddDays(_expirationOptions.RefreshToken)
        });
        if (await context.SaveChangesAsync() == 0)
            return null;

        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiresIn = _expirationOptions.AccessToken,
            RefreshToken = refreshToken
        };
        return accessTokenResponse;
    }

    public async Task<AccessTokenResponse?> ValidateExpiredTokens(
        string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal is null)
            return null;

        var user = await userManager.GetUserAsync(principal);
        if (user is null)
            return null;

        var refreshTokenEntity =
            await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Value == refreshToken);

        if (refreshTokenEntity is null || refreshTokenEntity.ExpiresAt <= timeProvider.GetUtcNow())
            return null;

        context.RefreshTokens.Remove(refreshTokenEntity);
        await context.SaveChangesAsync();

        return await GenerateTokens(user);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _symmetricSecurityKey,
            ValidateLifetime = false
        };
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token, tokenValidationParameters, out var securityToken);

        return securityToken is JwtSecurityToken jwtSecurityToken
               && jwtSecurityToken.Header.Alg.Equals(HmacSha256, InvariantCultureIgnoreCase)
            ? principal
            : null;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}