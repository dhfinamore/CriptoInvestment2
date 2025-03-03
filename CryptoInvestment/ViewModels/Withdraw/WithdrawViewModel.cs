using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Domain.InvOperations;

namespace CryptoInvestment.ViewModels.Withdraw;

public class WithdrawViewModel
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public int InvCurrencyId { get; set; }
    public List<InvBalance> InvBalances { get; set; } = [];
    public List<CustomerWithdrawalWallet> CustomerWithdrawalWallets { get; set; } = [];
}