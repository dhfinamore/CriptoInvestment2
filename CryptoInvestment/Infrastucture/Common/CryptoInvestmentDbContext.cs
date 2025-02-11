using System.Reflection;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.Common;

public class CryptoInvestmentDbContext : DbContext, IUnitOfWork
{
    public DbSet<Customer> Customers { get; init; } = null!;
    
    public DbSet<CustomerQuestion> CustomerQuestions { get; init; } = null!;
    
    public CryptoInvestmentDbContext(DbContextOptions<CryptoInvestmentDbContext> options) : base(options)
    {
    }

    public async Task CommitChangesAsync()
    {
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
