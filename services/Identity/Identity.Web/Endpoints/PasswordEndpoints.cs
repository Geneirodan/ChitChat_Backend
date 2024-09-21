using System.Text;
using System.Text.Encodings.Web;
using Identity.Persistence;
using Identity.Web.Requests;
using Identity.Web.Services;
using Identity.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Identity.Web.Endpoints;

internal static class PasswordEndpoints
{
    private const string GroupName = "password";
    internal static void MapPasswordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routeGroup = endpoints.MapGroup(GroupName).WithTags(GroupName);
        routeGroup.MapPost("/forgot", ForgotPasswordAsync);
        routeGroup.MapPost("/reset", ResetPasswordAsync).RequireAuthorization();
    }

    private static async Task<Results<Ok, ValidationProblem>> ResetPasswordAsync(
        [FromBody] ResetPasswordRequest request,
        [FromServices] UserManager<User> userManager
    )
    {
        var (email, resetCode, newPassword) = request;
        var user = await userManager.FindByEmailAsync(email);

        IdentityResult result;
        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetCode));
                result = await userManager.ResetPasswordAsync(user, code, newPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
            }
        }
        else
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email));

        return result.Succeeded ? TypedResults.Ok() : result.ToValidationProblem();
    }

    private static async Task<Results<Ok, ValidationProblem>> ForgotPasswordAsync(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IEmailSender emailSender
    )
    {
        var (email, resetUrl) = request;
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user)) 
            return TypedResults.Ok();

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var link = new StringBuilder()
            .Append(resetUrl)
            .Append("?email=")
            .Append(HtmlEncoder.Default.Encode(email))
            .Append("&code=")
            .Append(HtmlEncoder.Default.Encode(code))
            .ToString();
        await emailSender.SendPasswordResetCodeAsync(user.UserName!, email, link);
        return TypedResults.Ok();
    }
}