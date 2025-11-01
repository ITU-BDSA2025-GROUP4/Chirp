
namespace Chirp.Core.Interfaces;

public interface IEmailService {
    Task SendRegistrationConfirmationEmailAsync(string toEmail, string UserName, string confirmationLink);
    Task SendAccountCreatedEmailAsync(string toEmail, string UserName, string loginLink);
    Task SendResendConfirmationEmailAsync(string toEmail, string UserName, string confirmationLink);
}