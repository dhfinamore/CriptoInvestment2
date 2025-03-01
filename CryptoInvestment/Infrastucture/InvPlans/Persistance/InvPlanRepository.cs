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

    public async Task<List<InvPlan>> GetInvPlansAsync(int? customerId)
    {
        if (customerId is null)
        {
            return await _context.InvPlans.ToListAsync();
        }
        
        var invPlans = await _context.InvPlans
            .Where(plan => !_context.InvAssets
                        .Where(asset => asset.IdCustomer == customerId && !asset.Finalized)
                        .Select(asset => asset.IdInvPlans)
                        .Contains(plan.IdInvPlans))
            .ToListAsync();

        return invPlans;
    }

    public async Task<InvPlan?> GetInvPlanByIdAsync(int invPlaId)
    {
        return await _context.InvPlans.FirstOrDefaultAsync(plan => plan.IdInvPlans == invPlaId);
    }
}