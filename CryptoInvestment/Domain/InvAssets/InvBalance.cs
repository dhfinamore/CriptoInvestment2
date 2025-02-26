namespace CryptoInvestment.Domain.InvAssets;

public class InvBalance
{
    public int IdCustomer { get; set; }
    public int? IdCurrency { get; set; }
    public decimal? Balance { get; set; }
}