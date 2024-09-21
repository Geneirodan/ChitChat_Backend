using Identity.Emails.Services;
using Identity.Emails.ViewModels;
using Identity.Web.Options;
using Identity.Web.Resources;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Identity.Web.Services;

internal sealed class EmailSender(
    ILogger<EmailSender> logger,
    IOptions<EmailOptions> emailOptions,
    IStringLocalizer<Email> localizer,
    IRazorViewRenderer renderer) : IEmailSender
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    private async Task SendEmailAsync(string name, string email, string subject, string htmlMessage)
    {
        logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);
        try
        {
            var body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();
            var emailMessage = new MimeMessage { Subject = subject, Body = body };
            emailMessage.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.DefaultFromEmail));
            emailMessage.To.Add(new MailboxAddress(name, email));

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, useSsl: true);
            await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There was an error in sending mail");
        }
    }

    public async Task SendRegisterConfirmationAsync(string name, string email, string link)
    {
        var message = await renderer.RenderEmailConfirmationEmail(new EmailConfirmationViewModel(link));
        await SendEmailAsync(name, email, subject: localizer["RegisterConfirmation"], message);
        // return SendTemplate(email, code, returnUrl, "Register.html", localizer["Register"]);
    }

    public async Task SendPasswordResetCodeAsync(string name, string email, string link)
    {
        var message = await renderer.RenderForgotPasswordEmail(new ForgotPasswordViewModel(link));
        await SendEmailAsync(name, email, subject: localizer["PasswordReset"], message);
        // return SendTemplate(email, code, returnUrl, "PasswordReset.html", localizer["PasswordReset"]);
    }

    public async Task SendEmailChangeConfirmationAsync(string name, string email, string link)
    {
        var message = await renderer.RenderEmailConfirmationEmail(new EmailConfirmationViewModel(link));
        await SendEmailAsync(name, email, subject: localizer["EmailChange"], message);
        // return SendTemplate(email, code, returnUrl, "EmailChange.html", localizer["EmailChange"]);
    }

    // private async Task SendTemplate(string email, string code, string returnUrl, string filename, string subject)
    // {
    //     var path = Path.Combine(_emailOptions.TemplatesFolder, Thread.CurrentThread.CurrentCulture.Name, filename);
    //     var template = await File.ReadAllTextAsync(path);
    //     var link = $"{returnUrl}?email={HtmlEncoder.Default.Encode(email)}&code={HtmlEncoder.Default.Encode(code)}";
    //     var htmlMessage = string.Format(template, link);
    //     await SendEmailAsync(email, subject, htmlMessage);
    // }
    //
    // public 
}