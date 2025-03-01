using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.InvPlans;

namespace CryptoInvestment.ViewModels.Movements;

public class MovementsViewModel
{
    public int CustomerId { get; set; }
    public List<InvOperation> InvOperations { get; set; } = [];
    public List<InvPlan> InvPlans { get; set; } = [];
    public List<InvAction> InvActions { get; set; } = [];
}