using System.Net;
using System.Net.Mail;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Services.ConfigurationModels;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace CryptoInvestment.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task<ErrorOr<Success>> SendVerificationEmailAsync(string to, string subject, string body)
    {
        using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
        smtpClient.Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
        smtpClient.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Email),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            return Result.Success;
        }
        catch (SmtpException ex)
        {
            return ex.Message.Contains("Authentication Required") ? 
                Error.Failure(description: "Credenciales inválidas o autenticación requerida. Verifica tu configuración SMTP.") : 
                Error.Failure(ex.Message.Contains("5.7.0") ? "Problema con la autenticación SMTP. Asegúrate de que las credenciales y configuración sean correctas." 
                    : "Error general al enviar el correo.");
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "Ocurrió un error inesperado al enviar el correo.");
        }
    }
}
