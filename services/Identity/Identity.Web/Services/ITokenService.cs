using Identity.Persistence;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Identity.Web.Services;

internal interface ITokenService
{
    Task<AccessTokenResponse?> GenerateTokens(User user);
    Task<AccessTokenResponse?> ValidateExpiredTokens(string accessToken, string refreshToken);
}