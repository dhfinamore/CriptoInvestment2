using CryptoInvestment.Domain.Customers;

namespace CryptoInvestment.Application.Common.Interface;

public interface ICustomerBeneficiaryRepository
{
    public Task AddBeneficiaryAsync(CustomerBeneficiary beneficiary);
    public Task<List<CustomerBeneficiary>> GetBeneficiaryByCustomerIdAsync(int customerId);
    public Task UpdateCustomerBeneficiaryAsync(List<CustomerBeneficiary> customerBeneficiaries);
    public Task<CustomerBeneficiary?> GetBeneficiaryByIdAsync(int beneficiaryId);
    public Task DeleteBeneficiaryAsync(CustomerBeneficiary beneficiary);
}