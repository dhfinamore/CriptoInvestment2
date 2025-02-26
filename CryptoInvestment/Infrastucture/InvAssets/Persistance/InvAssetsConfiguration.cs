using CryptoInvestment.Domain.InvAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.InvAssets.Persistance;

public class InvAssetConfiguration : IEntityTypeConfiguration<InvAsset>
{
    public void Configure(EntityTypeBuilder<InvAsset> builder)
    {
        builder.ToTable("inv_assets");

        builder.HasKey(a => a.IdInvAssets);
        builder.Property(a => a.IdInvAssets)
            .HasColumnName("id_inv_assets")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.IdCustomer)
            .HasColumnName("id_customer");

        builder.Property(a => a.IdInvPlans)
            .HasColumnName("id_inv_plans");

        builder.Property(a => a.IdCurrency)
            .HasColumnName("id_currency");

        builder.Property(a => a.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(12,2)");

        builder.Property(a => a.ExpectedProfit)
            .HasColumnName("expected_profit")
            .HasColumnType("decimal(12,2)");

        builder.Property(a => a.DateStart)
            .HasColumnName("date_start")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.DateEnd)
            .HasColumnName("date_end");

        builder.Property(a => a.Finalized)
            .HasColumnName("finalized")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(false);
    }
}
