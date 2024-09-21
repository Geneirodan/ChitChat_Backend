using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Identity.Web.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Identity.Web.Extensions;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
internal static class RuleBuilderOptionsExtensions
{
    internal static IRuleBuilderOptions<T, string>
        IsValidUsername<T>(this IRuleBuilderInitial<T, string> ruleBuilder) =>
        ruleBuilder.Cascade(CascadeMode.Stop).NotEmpty().Length(3, 20);

    internal static IRuleBuilderOptions<T, string> IsValidEmail<T>(this IRuleBuilderInitial<T, string> ruleBuilder) =>
        ruleBuilder.Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(byte.MaxValue);

    internal static IRuleBuilderOptions<T, string> IsValidPassword<T>(this IRuleBuilderInitial<T, string> ruleBuilder,
        PasswordOptions passwordOptions)
    {
        var builderOptions = ruleBuilder.NotEmpty();

        if (passwordOptions.RequireDigit)
            builderOptions
                .Must(x => x.Any(c => c is >= '0' and <= '9'))
                .WithMessage(ErrorMessages.Password_Digit);

        if (passwordOptions.RequireLowercase)
            builderOptions
                .Must(x => x.Any(c => c is >= 'a' and <= 'z'))
                .WithMessage(ErrorMessages.Password_LowerCaseCharacter);

        if (passwordOptions.RequireUppercase)
            builderOptions
                .Must(x => x.Any(c => c is >= 'A' and <= 'Z'))
                .WithMessage(ErrorMessages.Password_UpperCaseCharacter);

        if (passwordOptions.RequireNonAlphanumeric)
            builderOptions
                .Must(x => !x.All(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9'))
                .WithMessage(ErrorMessages.Password_NonAlphanumericCharacter);


        return builderOptions.MinimumLength(passwordOptions.RequiredLength);
    }
}