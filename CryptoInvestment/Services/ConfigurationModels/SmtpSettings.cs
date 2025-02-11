namespace CryptoInvestment.Services.ConfigurationModels;

public class SmtpSettings
{
    public const string Section = "Smtp";

    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}