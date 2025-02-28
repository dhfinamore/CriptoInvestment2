using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Infrastucture.Common;

using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.InvAssets.Persistance;

public class InvAssetsRepository : IInvAssetsRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public InvAssetsRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvBalance>> GetInvBalances(int customerId)
    {
        return await _context.InvBalances.Where(c => c.IdCustomer == customerId).ToListAsync();
    }

    public async Task CreateInvAssetsAsync(InvAsset invAsset)
    {
        await _context.InvAssets.AddAsync(invAsset);
    }

    public Task UpdateInvBalance(InvBalance invBalance)
    {
        _context.InvBalances.Update(invBalance);
        return Task.CompletedTask;
    }
}