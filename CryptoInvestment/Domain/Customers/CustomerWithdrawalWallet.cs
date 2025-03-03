namespace CryptoInvestment.Domain.Customers;

public class CustomerWithdrawalWallet
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string WalletName { get; set; } = null!;
    public int? InvCurrency { get; set; }
    public string WalletAccount { get; set; } = null!;
    public bool Used { get; set; }
}
