using Identity.Emails.ViewModels;

namespace Identity.Emails.Services;

public interface IRazorViewRenderer
{
    Task<string> RenderEmailConfirmationEmail(EmailConfirmationViewModel model);
    Task<string> RenderForgotPasswordEmail(ForgotPasswordViewModel model);
}