using CryptoInvestment.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerQuestionConfiguration : IEntityTypeConfiguration<CustomerQuestion>
{
    public void Configure(EntityTypeBuilder<CustomerQuestion> builder)
    {
        builder.ToTable("customer_questions");

        builder.HasKey(q => q.IdCustomerQuestions);

        builder.Property(q => q.IdCustomerQuestions)
            .HasColumnName("id_customer_questions")
            .ValueGeneratedOnAdd();

        builder.Property(q => q.IdCustomer)
            .HasColumnName("id_customer")
            .IsRequired();

        builder.Property(q => q.IdQuestion)
            .HasColumnName("id_question")
            .IsRequired();

        builder.Property(q => q.Response)
            .HasColumnName("response")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(q => q.Order)
            .HasColumnName("order")
            .IsRequired();
    }
}
