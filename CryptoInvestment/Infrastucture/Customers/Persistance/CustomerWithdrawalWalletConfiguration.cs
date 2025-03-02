using CryptoInvestment.Domain.Customers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerWithdrawalsWalletConfiguration : IEntityTypeConfiguration<CustomerWithdrawalWallet>
{
    public void Configure(EntityTypeBuilder<CustomerWithdrawalWallet> builder)
    {
        builder.ToTable("customer_withdrawals_wallet");

        builder.HasKey(e => e.Id)
            .HasName("PRIMARY");

        builder.Property(e => e.Id)
            .HasColumnName("id_customer_withdrawals_wallet")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CustomerId)
            .HasColumnName("id_customer")
            .IsRequired();

        builder.Property(e => e.WalletName)
            .HasColumnName("wallet_name")
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(e => e.InvCurrency)
            .HasColumnName("inv_currency")
            .IsRequired(false);

        builder.Property(e => e.WalletAccount)
            .HasColumnName("wallet_account")
            .HasMaxLength(200)
            .IsUnicode(false)
            .IsRequired(false);

        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("id_customer");
    }
}
