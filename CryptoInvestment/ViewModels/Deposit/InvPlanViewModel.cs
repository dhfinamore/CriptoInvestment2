using CryptoInvestment.Domain.InvPlan;

namespace CryptoInvestment.ViewModels.Deposit;

public class InvPlanViewModel
{
    public int CustomerId { get; set; }
    public List<InvPlan> InvPlans = [];
}