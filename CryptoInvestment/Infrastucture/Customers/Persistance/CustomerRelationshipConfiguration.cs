using CryptoInvestment.Domain.Customers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerRelationshipConfiguration : IEntityTypeConfiguration<CustomerRelationship>
{
    public void Configure(EntityTypeBuilder<CustomerRelationship> builder)
    {
        builder.ToTable("customer_rela");

        builder.HasKey(cr => cr.RelationshipId);

        builder.Property(cr => cr.RelationshipId)
            .HasColumnName("id_customer_rela")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(cr => cr.Relationship)
            .HasColumnName("relation")
            .HasMaxLength(45)
            .IsUnicode(false);
    }
}