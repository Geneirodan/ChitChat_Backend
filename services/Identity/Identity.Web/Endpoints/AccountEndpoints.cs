using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Identity.Persistence;
using Identity.Web.Requests;
using Identity.Web.Responses;
using Identity.Web.Services;
using Identity.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Web.Endpoints;

using InfoResponse = Results<Ok<UserViewModel>, ValidationProblem, UnauthorizedHttpResult>;
using ChangeEmailResponse = Results<Ok, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, Conflict>;
using ChangePasswordResponse = Results<Ok, ValidationProblem, UnauthorizedHttpResult>;
using ChangeEmailConfirmedResponse = Results<Ok, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, Conflict>;

internal static class AccountEndpoints
{
    private const string GroupName = "account";

    internal static void MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var accountGroup = endpoints.MapGroup(GroupName).WithTags(GroupName).RequireAuthorization();

        accountGroup.MapGet("/", InfoAsync);
        accountGroup.MapPatch("/email", ChangeEmailAsync);
        accountGroup.MapPatch("/email/confirm", ChangeEmailConfirmedAsync);
        accountGroup.MapPatch("/password", ChangePasswordAsync);
    }


    private static async Task<ChangePasswordResponse> ChangePasswordAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] ChangePasswordRequest request,
        [FromServices] UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            return TypedResults.Unauthorized();

        var (newPassword, oldPassword) = request;

        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        return result.Succeeded ? TypedResults.Ok() : result.ToValidationProblem();
    }

    private static async Task<ChangeEmailResponse> ChangeEmailAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] ChangeEmailRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IEmailSender emailSender
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            return TypedResults.Unauthorized();

        var (newEmail, returnUrl) = request;

        if (await userManager.FindByEmailAsync(newEmail) is not null)
            return TypedResults.Forbid();

        var email = await userManager.GetEmailAsync(user) ?? throw new NullReferenceException("User.Email is null");
        if (newEmail.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            return TypedResults.Conflict();

        var code = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var link = new StringBuilder()
            .Append(returnUrl)
            .Append("?email=")
            .Append(HtmlEncoder.Default.Encode(email))
            .Append("&code=")
            .Append(HtmlEncoder.Default.Encode(code))
            .ToString();
        await emailSender.SendEmailChangeConfirmationAsync(user.UserName!, email, link);
        return TypedResults.Ok();
    }

    private static async Task<InfoResponse> InfoAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromServices] UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
            return TypedResults.Unauthorized();

        var info = new UserViewModel(user);
        return TypedResults.Ok(info);
    }


    private static async Task<ChangeEmailConfirmedResponse> ChangeEmailConfirmedAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] ChangeEmailConfirmedRequest request,
        [FromServices] UserManager<User> userManager
    )
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            return TypedResults.Unauthorized();

        var (newEmail, confirmationToken) = request;

        if (await userManager.FindByEmailAsync(newEmail) is not null)
            return TypedResults.Forbid();

        var email = await userManager.GetEmailAsync(user) ?? throw new NullReferenceException("User.Email is null");
        if (newEmail.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            return TypedResults.Conflict();
        
        var result = await userManager.ChangeEmailAsync(user, newEmail, confirmationToken);
        return result.Succeeded ? TypedResults.Ok() : result.ToValidationProblem();
    }
}