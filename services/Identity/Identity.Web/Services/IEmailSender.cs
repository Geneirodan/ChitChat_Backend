namespace Identity.Web.Services;

internal interface IEmailSender
{
    Task SendRegisterConfirmationAsync(string name, string email, string link);
    Task SendPasswordResetCodeAsync(string name, string email, string link);
    Task SendEmailChangeConfirmationAsync(string name, string email, string link);
}