using CryptoInvestment.Domain.Customers;

namespace CryptoInvestment.Application.Common.Interface;

public interface ICustomerRepository
{
    public Task CreateCustomerAsync(Customer customer);
    public Task<Customer?> GetCustomerByEmailAsync(string email);
    public Task<Customer?> GetCustomerByIdAsync(int idCustomer);
    public Task<bool> HasSecurityQuestionsAsync(int idCustomer);
    public Task UpdateCustomerAsync(Customer customer);
    public Task<bool> ExistEmailAsync(string email);
    public Task<bool> ExistPhoneAsync(string phone);
}