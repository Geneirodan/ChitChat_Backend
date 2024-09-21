using FluentValidation;
using Identity.Web.Extensions;

namespace Identity.Web.Requests;

internal sealed record ResendEmailRequest(string Email, string ConfirmUrl)
{
    internal sealed class Validator : AbstractValidator<ResendEmailRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Email).IsValidEmail();
            RuleFor(x => x.ConfirmUrl).NotEmpty();
        }
    }
}