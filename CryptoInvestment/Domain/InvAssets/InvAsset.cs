namespace CryptoInvestment.Domain.InvAssets;

public class InvAsset
{
    public int IdInvAssets { get; set; }
    public int? IdCustomer { get; set; }
    public int? IdInvPlans { get; set; }
    public int? IdCurrency { get; set; }
    public decimal? Amount { get; set; }
    public decimal? ExpectedProfit { get; set; }
    public decimal Withdrawals { get; set; }
    public decimal Balance { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public bool Finalized { get; set; }
}
