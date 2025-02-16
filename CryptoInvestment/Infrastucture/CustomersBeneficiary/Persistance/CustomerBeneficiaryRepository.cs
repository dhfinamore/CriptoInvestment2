using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Infrastucture.Common;

using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.CustomersBeneficiary.Persistance;

public class CustomerBeneficiaryRepository : ICustomerBeneficiaryRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public CustomerBeneficiaryRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task AddBeneficiaryAsync(CustomerBeneficiary beneficiary)
    {
        await _context.CustomerBeneficiaries.AddAsync(beneficiary);
    }

    public async Task<List<CustomerBeneficiary>> GetBeneficiaryByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerBeneficiaries
            .Where(b => b.IdCustomer == customerId).ToListAsync();
    }

    public Task UpdateCustomerBeneficiaryAsync(List<CustomerBeneficiary> customerBeneficiaries)
    {
        _context.CustomerBeneficiaries.UpdateRange(customerBeneficiaries);
        return Task.CompletedTask;
    }

    public async Task<CustomerBeneficiary?> GetBeneficiaryByIdAsync(int beneficiaryId)
    {
        return await _context.CustomerBeneficiaries
            .FirstOrDefaultAsync(b => b.IdCustomerBeneficiary == beneficiaryId);
    }

    public Task DeleteBeneficiaryAsync(CustomerBeneficiary beneficiary)
    {
        _context.CustomerBeneficiaries.Remove(beneficiary);
        return Task.CompletedTask;
    }
}