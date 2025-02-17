using CryptoInvestment.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerPicConfiguration : IEntityTypeConfiguration<CustomerPic>
{
    public void Configure(EntityTypeBuilder<CustomerPic> builder)
    {
        builder.ToTable("customer_pic");
        
        builder.HasKey(cp => cp.IdCustomerPic);

        builder.Property(cp => cp.IdCustomerPic)
            .HasColumnName("idcustomer_pic")
            .ValueGeneratedOnAdd();

        builder.Property(cp => cp.IdCustomer)
            .HasColumnName("id_customer")
            .IsRequired();

        builder.Property(cp => cp.Type)
            .HasColumnName("type")
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(cp => cp.PictureFront)
            .HasColumnName("picture_front")
            .IsRequired();

        builder.Property(cp => cp.PictureBack)
            .HasColumnName("picture_back")
            .IsRequired(false);
        
        builder.Property(cp => cp.RejectMessage)
            .HasColumnName("reject_message")
            .IsRequired(false);
    }
}