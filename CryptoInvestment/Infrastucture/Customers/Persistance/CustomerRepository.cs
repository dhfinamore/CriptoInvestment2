using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.SecurityQuestions;
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

    public async Task AddSecurityQuestions(List<CustomerQuestion> securityQuestions)
    {
        await _context.CustomerQuestions.AddRangeAsync(securityQuestions);
    }

    public async Task UpdateSecurityQuestions(int customerId, List<CustomerQuestion> securityQuestions)
    {
        var customerQuestions = await _context.CustomerQuestions
            .Where(cq => cq.IdCustomer == customerId)
            .OrderBy(cq => cq.Order).ToListAsync();
        
        for (int i = 0; i < securityQuestions.Count; i++)
        {
            customerQuestions[i].IdQuestion = securityQuestions[i].IdQuestion;
            customerQuestions[i].Response = securityQuestions[i].Response;
        }
        
        _context.UpdateRange(customerQuestions);
    }

    public async Task<List<CustomerQuestion>> GetSecurityQuestions(int customerId)
    {
        return await _context.CustomerQuestions
            .Where(cq => cq.IdCustomer == customerId)
            .OrderBy(cq => cq.Order).ToListAsync();
    }

    public Task<CustomerPic?> GetCustomerPic(int customerId)
    {
        return _context.CustomerPics.FirstOrDefaultAsync(cp => cp.IdCustomer == customerId);
    }

    public Task UpdateCustomerPic(CustomerPic customerPic)
    {
        _context.CustomerPics.Update(customerPic);
        return Task.CompletedTask;
    }

    public async Task CreateCustomerPicAsync(CustomerPic customerPic)
    {
        await _context.CustomerPics.AddAsync(customerPic);
    }

    public Task DeleteCustomerPic(CustomerPic customerPic)
    {
        _context.CustomerPics.Remove(customerPic);
        return Task.CompletedTask;
    }

    public async Task<List<CustomerRelationship>> GetCustomerRelationships()
    {
        return await _context.CustomerRelationships.ToListAsync();
    }
}