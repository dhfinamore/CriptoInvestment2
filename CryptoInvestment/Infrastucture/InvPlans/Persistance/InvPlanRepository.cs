using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvPlans;
using CryptoInvestment.Infrastucture.Common;

using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.InvPlans.Persistance;

public class InvPlanRepository : IInvPlanRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public InvPlanRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvPlan>> GetInvPlansAsync()
    {
        return await _context.InvPlans.ToListAsync();
    }

    public async Task<InvPlan?> GetInvPlanByIdAsync(int invPlaId)
    {
        return await _context.InvPlans.FirstOrDefaultAsync(plan => plan.IdInvPlans == invPlaId);
    }
}