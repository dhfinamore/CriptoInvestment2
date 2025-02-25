using CryptoInvestment.Domain.InvPlans;

namespace CryptoInvestment.Application.Common.Interface;

public interface IInvPlanRepository
{
    public Task<List<InvPlan>> GetInvPlansAsync();
    public Task<InvPlan?> GetInvPlanByIdAsync(int invPlaId);
}