using System.Security.Claims;
using CryptoInvestment.Domain.Customers;

namespace CryptoInvestment.Application.Common.Interface;

public interface ICustomerAuthenticationService
{
    public Task<ClaimsIdentity> CreateClaimsIdentityAsync(Customer customer);
}