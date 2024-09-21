using FluentValidation;
using Identity.Web.Extensions;
using Identity.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Identity.Web.Requests;


public sealed record ForgotPasswordRequest(string Email, string ResetUrl)
{
    internal sealed class Validator : AbstractValidator<ForgotPasswordRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Email).IsValidEmail();
            RuleFor(x => x.ResetUrl).NotEmpty();
        }
    }
}