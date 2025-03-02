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
    public Task AddSecurityQuestions(List<CustomerQuestion> securityQuestions);
    public Task UpdateSecurityQuestions(int customerId, List<CustomerQuestion> securityQuestions);
    public Task<List<CustomerQuestion>> GetSecurityQuestions(int customerId);
    public Task<CustomerPic?> GetCustomerPic(int customerId);
    public Task UpdateCustomerPic(CustomerPic customerPic);
    public Task CreateCustomerPicAsync(CustomerPic customerPic);
    public Task DeleteCustomerPic(CustomerPic customerPic);
    public Task<List<CustomerRelationship>> GetCustomerRelationships();
    public Task<List<Customer>> GetCustomerReferrals(int customerId);
    public Task<List<CustomerWithdrawalWallet>> GetCustomerWithdrawalWallets(int customerId);
}