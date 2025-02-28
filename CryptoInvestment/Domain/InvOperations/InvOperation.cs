namespace CryptoInvestment.Domain.InvOperations;

public class InvOperation
{
    public int IdInvOperations { get; set; }
    public int IdCustomer { get; set; }
    public int IdCurrency { get; set; }
    public decimal Amount { get; set; }
    public int? IdInvPlans { get; set; }
    public int? IdInvAssets { get; set; }
    public DateTime Date { get; set; }
    public int IdInvAction { get; set; }
    public string? IdTransaction { get; set; }
    public int Status { get; set; }
}
