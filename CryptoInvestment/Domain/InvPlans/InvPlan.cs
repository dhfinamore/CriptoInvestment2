namespace CryptoInvestment.Domain.InvPlan;

public class InvPlan
{
    public int IdInvPlans { get; set; }
    public string PlanName { get; set; } = null!;
    public string MonthsInvested { get; set; } = null!;
    public decimal? ProfitPercentage { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}
