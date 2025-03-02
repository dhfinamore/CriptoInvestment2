using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Infrastucture.Common;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.InvOperations.Persistance;

public class InvOperationRepository : IInvOperationRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public InvOperationRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task CreateInvOperationAsync(InvOperation invOperation)
    {
        await _context.InvOperations.AddAsync(invOperation);
    }

    public async Task<List<InvOperation>> GetInvOperationsAsync(int customerId)
    {
        return await _context.InvOperations.
            Where(io => io.IdCustomer == customerId).
            ToListAsync();
    }

    public async Task<InvOperation?> GetInvOperationByIdAsync(int id)
    {
        return await _context.InvOperations.FirstOrDefaultAsync(io => io.IdInvOperations == id);
    }

    public async Task<List<InvAction>> GetInvActionsAsync()
    {
        return await _context.InvActions.ToListAsync();
    }

    public async Task<List<InvCurrency>> GetInvCurrenciesAsync()
    {
        return await _context.InvCurrencies.ToListAsync();
    }
}
