using System.Text;
using Identity.Persistence;
using Identity.Web.Extensions;
using Identity.Web.Requests;
using Identity.Web.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.String;

namespace Identity.Web.Endpoints;

using RegisterResponse = Results<Created, ValidationProblem>;
using LoginResponse = Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, ForbidHttpResult>;
using RefreshResponse = Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult>;

internal static class AuthEndpoints
{
    private const string GroupName = "auth";

    internal static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routeGroup = endpoints.MapGroup(GroupName).WithTags(GroupName);
        routeGroup.MapPost("/register", RegisterAsync);
        routeGroup.MapPost("/login", LoginAsync);
        routeGroup.MapPost("/refresh", RefreshAsync);
    }

    private static async Task<RefreshResponse> RefreshAsync(
        [FromBody] RefreshRequest request,
        [FromServices] ITokenService tokenService
    )
    {
        var (accessToken, refreshToken) = request;
        var tokens = await tokenService.ValidateExpiredTokens(accessToken, refreshToken);
        return tokens is null 
            ? TypedResults.Unauthorized() 
            : TypedResults.Ok(tokens);
    }

    private static async Task<LoginResponse> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] SignInManager<User> signInManager,
        [FromServices] ITokenService tokenService)
    {
        var (username, password, twoFactorCode, twoFactorRecoveryCode) = request;
        var userManager = signInManager.UserManager;

        var user = await userManager.FindByEmailAsync(username) ?? await userManager.FindByNameAsync(username);
        if (user is null)
            return TypedResults.Unauthorized();

        var result = await signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return TypedResults.Forbid();
        
        if (result.IsNotAllowed)
            return TypedResults.Forbid();

        if (result.RequiresTwoFactor)
        {
            if (!IsNullOrEmpty(twoFactorCode))
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(twoFactorCode, true, true);
            else if (!IsNullOrEmpty(twoFactorRecoveryCode))
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(twoFactorRecoveryCode);
        }

        if (!result.Succeeded) 
            return TypedResults.Unauthorized();
        
        var tokens = await tokenService.GenerateTokens(user);
        return tokens is null 
            ? TypedResults.Unauthorized() 
            : TypedResults.Ok(tokens);
    }

    private static async Task<RegisterResponse> RegisterAsync(
        [FromBody] RegisterRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IEmailSender emailSender)
    {
        var (username, email, password, returnUrl) = request;

        var user = new User { Email = email, UserName = username };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return result.ToValidationProblem();

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = new StringBuilder()
            .Append(returnUrl)
            .Append("?email=")
            .Append(email)
            .Append("&code=")
            .Append(code)
            .ToString();
        await emailSender.SendRegisterConfirmationAsync(username, email, link);

        return TypedResults.Created();
    }
}