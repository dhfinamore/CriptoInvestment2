using CryptoInvestment.Domain.InvPlan;

namespace CryptoInvestment.Application.Common.Interface;

public interface IInvPlanRepository
{
    public Task<List<InvPlan>> GetInvPlansAsync();
    public Task<InvPlan?> GetInvPlanByIdAsync(int invPlaId);
}