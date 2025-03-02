using System.Reflection;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.InvPlans;
using CryptoInvestment.Domain.SecurityQuestions;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.Common;

public class CryptoInvestmentDbContext : DbContext, IUnitOfWork
{
    public DbSet<Customer> Customers { get; init; } = null!;
    public DbSet<CustomerQuestion> CustomerQuestions { get; init; } = null!;
    public DbSet<SecurityQuestion> SecurityQuestions { get; init; } = null!;
    public DbSet<CustomerBeneficiary> CustomerBeneficiaries { get; init; } = null!;
    public DbSet<CustomerPic> CustomerPics { get; init; } = null!;
    public DbSet<CustomerRelationship> CustomerRelationships { get; init; } = null!;
    public DbSet<InvPlan> InvPlans { get; init; } = null!;
    public DbSet<InvCurrency> InvCurrencies { get; init; } = null!;
    public DbSet<InvOperation> InvOperations { get; init; } = null!;
    public DbSet<InvAction> InvActions { get; init; } = null!;
    public DbSet<InvAsset> InvAssets { get; init; } = null!;
    public DbSet<InvBalance> InvBalances { get; init; } = null!;
    public DbSet<CustomerWithdrawalWallet> CustomerWithdrawalWallets { get; init; } = null!;
    
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
