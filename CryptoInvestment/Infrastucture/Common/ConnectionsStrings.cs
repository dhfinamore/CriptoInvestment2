namespace CryptoInvestment.Infrastucture.Common;

public class ConnectionSettings
{
    public const string Section = "ConnectionStrings";

    public string CryptoInvestmentDb { get; set; } = null!;
}