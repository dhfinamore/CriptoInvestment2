using System.Security.Claims;

using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;

using Microsoft.AspNetCore.Authentication.Cookies;

namespace CryptoInvestment.Infrastucture.Authentication;

public class CustomerAuthenticationService : ICustomerAuthenticationService
{
    public Task<ClaimsIdentity> CreateClaimsIdentityAsync(Customer customer)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.IdCustomer.ToString()),
            new Claim(ClaimTypes.Name, customer.Nombre ?? "Usuario"),
            new Claim(ClaimTypes.Email, customer.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return Task.FromResult(identity);
    }
}