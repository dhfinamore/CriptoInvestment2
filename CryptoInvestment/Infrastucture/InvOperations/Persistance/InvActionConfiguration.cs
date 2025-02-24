namespace CryptoInvestment.Infrastucture.InvOperations.Persistance;

using CryptoInvestment.Domain.InvOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InvActionConfiguration : IEntityTypeConfiguration<InvAction>
{
    public void Configure(EntityTypeBuilder<InvAction> builder)
    {
        builder.ToTable("inv_actions");

        builder.HasKey(ia => ia.IdInvActions);

        builder.Property(ia => ia.IdInvActions)
            .HasColumnName("id_inv_actions")
            .ValueGeneratedOnAdd();

        builder.Property(ia => ia.Action)
            .HasColumnName("action")
            .HasMaxLength(45);
    }
}
