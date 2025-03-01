using CryptoInvestment.Domain.InvOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.InvOperations.Persistance;

public class InvOperationConfiguration : IEntityTypeConfiguration<InvOperation>
{
    public void Configure(EntityTypeBuilder<InvOperation> builder)
    {
        builder.ToTable("inv_operations");
        
        builder.HasKey(io => io.IdInvOperations);
        
        builder.Property(io => io.IdInvOperations)
            .HasColumnName("id_inv_operations")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(io => io.IdCustomer)
            .HasColumnName("id_customer")
            .IsRequired();

        builder.Property(io => io.IdCurrency)
            .HasColumnName("id_currency")
            .IsRequired();

        builder.Property(io => io.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(io => io.IdInvPlans)
            .HasColumnName("id_inv_plans")
            .IsRequired(false);
        
        builder.Property(io => io.IdInvAssets)
            .HasColumnName("id_inv_assets")
            .IsRequired(false);

        builder.Property(io => io.Date)
            .HasColumnName("date")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(io => io.IdInvAction)
            .HasColumnName("id_inv_actions")
            .IsRequired();
        
        builder.Property(io => io.IdTransaction)
            .HasColumnName("id_transaction")
            .IsRequired(false)
            .HasColumnType("varchar(150)");
        
        builder.Property(io => io.Status)
            .HasColumnName("status")
            .IsRequired();
    }
}
