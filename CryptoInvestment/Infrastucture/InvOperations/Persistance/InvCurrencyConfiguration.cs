using CryptoInvestment.Domain.InvOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.InvOperations.Persistance;

public class InvCurrencyConfiguration : IEntityTypeConfiguration<InvCurrency>
{
    public void Configure(EntityTypeBuilder<InvCurrency> builder)
    {
        builder.ToTable("inv_currency");

        builder.HasKey(e => e.CurrencyId);

        builder.Property(e => e.CurrencyId)
            .HasColumnName("id_currency")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Description)
            .HasColumnName("descrip")
            .HasMaxLength(45);

        builder.Property(e => e.WalletAddress)
            .HasColumnName("ghr_deposit_walletaddr")
            .HasMaxLength(200);
    }
}