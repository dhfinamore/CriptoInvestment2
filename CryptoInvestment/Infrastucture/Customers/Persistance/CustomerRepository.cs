using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Infrastucture.Common;

using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerRepository : ICustomerRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public CustomerRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetCustomerByIdAsync(int idCustomer)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.IdCustomer == idCustomer);
    }

    public async Task<bool> HasSecurityQuestionsAsync(int idCustomer)
    {
        return await _context.CustomerQuestions.AnyAsync(sq => sq.IdCustomer == idCustomer);
    }

    public Task UpdateCustomerAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public Task<bool> ExistEmailAsync(string email)
    {
        return _context.Customers.AnyAsync(c => c.Email == email);
    }

    public Task<bool> ExistPhoneAsync(string phone)
    {
        return _context.Customers.AnyAsync(c => c.Phone == phone);
    }
}