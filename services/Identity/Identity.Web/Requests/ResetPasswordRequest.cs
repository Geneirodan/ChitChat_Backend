using FluentValidation;
using Identity.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Web.Requests;


internal sealed record ResetPasswordRequest(string Email, string ResetCode, string NewPassword)
{
    internal class Validator : AbstractValidator<ResetPasswordRequest>
    {
        public Validator(IOptions<IdentityOptions> options)
        {
            RuleFor(x => x.Email).IsValidEmail();
            RuleFor(x => x.ResetCode).NotEmpty();
            RuleFor(x => x.NewPassword).IsValidPassword(options.Value.Password);
        }
    }
}