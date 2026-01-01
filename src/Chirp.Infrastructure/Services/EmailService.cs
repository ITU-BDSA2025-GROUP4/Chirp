using System.Net;
using System.Net.Mail;
using Chirp.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure.Services;

// NOTE: The content and style of the emails have been copied form https://dotnettutorials.net/lesson/register-login-logout-in-asp-net-core-identity/
//
// This is unused
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendRegistrationConfirmationEmailAsync(
        string toEmail,
        string UserName,
        string confirmationLink
    )
    {
        //TODO: Change style of email
        string htmlContent =
            $@"
            <html><body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                    <h2 style='color:#333;'>Welcome, {UserName}!</h2>
                    <p style='font-size:16px; color:#555;'>Thank you for registering. Please confirm your email by clicking the buttom below.</p>
                    <p style='text-align:center;'>
                        <a href='{confirmationLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Confirm Your Email</a>
                    </p>
                    <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} ITU-BDSA2025-GROUP4 </p>
                </div>
            </body></html>";
        await SendEmailAsync(toEmail, "Email Confirmation - Chirp", htmlContent, true);
    }

    async Task IEmailService.SendAccountCreatedEmailAsync(
        string toEmail,
        string UserName,
        string loginLink
    )
    {
        //TODO: Change style of email
        string htmlContent =
            $@"
                <html><body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                  <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                    <h2 style='color:#333;'>Hello, {UserName}!</h2>
                    <p style='font-size:16px; color:#555;'>Your account has been successfully created and your email is confirmed.</p>
                    <p style='text-align:center;'>
                      <a href='{loginLink}' style='background:#198754; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Login to Your Account</a>
                    </p>
                    <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} ITU-BDSA2025-GROUP4</p>
                  </div>
                </body></html>";
        await SendEmailAsync(toEmail, "Account created - Chirp", htmlContent, true);
    }

    async Task IEmailService.SendResendConfirmationEmailAsync(
        string toEmail,
        string UserName,
        string confirmationLink
    )
    {
        //TODO: Change style of email
        string htmlContent =
            $@"
                <html><body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                  <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                    <h2 style='color:#333;'>Hello, {UserName}!</h2>
                    <p style='font-size:16px; color:#555;'>You requested a new email confirmation link. Please confirm your email by clicking the button below.</p>
                    <p style='text-align:center;'>
                      <a href='{confirmationLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Confirm Your Email</a>
                    </p>
                    <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} ITU-BDSA2025-GROUP4</p>
                  </div>
                </body></html>";
        await SendEmailAsync(toEmail, "Email confirmation - Chirp", htmlContent, true);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        bool isBodyHtml = false
    )
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml,
            };
            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true,
            };

            await client.SendMailAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}