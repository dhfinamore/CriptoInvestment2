using CryptoInvestment.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.CustomersBeneficiary.Persistance;

public class CustomerBeneficiaryConfigurations : IEntityTypeConfiguration<CustomerBeneficiary>
{
    public void Configure(EntityTypeBuilder<CustomerBeneficiary> builder)
    {
        builder.ToTable("customer_ben");
        
        builder.HasKey(cb => cb.IdCustomerBeneficiary);
        
        builder.Property(cb => cb.IdCustomerBeneficiary)
            .HasColumnName("id_cb")
            .ValueGeneratedOnAdd();
        
        builder.Property(cb => cb.IdCustomer)
            .HasColumnName("id_customer")
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(cb => cb.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(45)
            .IsRequired();
        
        builder.Property(cb => cb.ApePat)
            .HasColumnName("ape_pat")
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(cb => cb.ApeMat)
            .HasColumnName("ape_mat")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(cb => cb.Tel)
            .HasColumnName("tel")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(cb => cb.RelationshipId)
            .HasColumnName("id_customer_rela")
            .IsRequired(true);

        builder.Property(cb => cb.Porcent)
            .HasColumnName("porce")
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);
    }
}