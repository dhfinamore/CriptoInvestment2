namespace CryptoInvestment.Domain.InvOperations;

public class InvCurrency
{
    public int CurrencyId { get; set; }
    public string Description { get; set; } = null!;
    public string WalletAddress { get; set; } = null!;
}