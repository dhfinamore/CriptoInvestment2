using CryptoInvestment.Domain.InvOperations;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class CustomerWithdrawalWalletsViewModel
{
    public int? WalletId { get; set; }
    public string WalletName { get; set; } = null!;
    public int InvCurrency { get; set; }
    public string WalletAccount { get; set; } = null!;
    public List<InvCurrency> InvCurrencies { get; set; } = [];
    public bool Used { get; set; } = false;
}