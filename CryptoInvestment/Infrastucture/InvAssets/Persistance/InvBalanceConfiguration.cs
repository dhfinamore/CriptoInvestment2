using CryptoInvestment.Domain.InvAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.InvAssets.Persistance;

public class InvBalanceConfiguration: IEntityTypeConfiguration<InvBalance>
{
    public void Configure(EntityTypeBuilder<InvBalance> builder)
    {
        builder.ToTable("inv_balance");

        builder.HasKey(e => e.IdCustomer)
            .HasName("PRIMARY");

        builder.Property(e => e.IdCustomer)
            .HasColumnName("id_customer")
            .HasColumnType("int")
            .IsRequired()
            .UseMySqlIdentityColumn();

        builder.Property(e => e.IdCurrency)
            .HasColumnName("id_currency")
            .HasColumnType("int")
            .IsRequired(false);

        builder.Property(e => e.Balance)
            .HasColumnName("balance")
            .HasColumnType("decimal(12,2)")
            .IsRequired(false);
            
        builder.HasAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
    }
}