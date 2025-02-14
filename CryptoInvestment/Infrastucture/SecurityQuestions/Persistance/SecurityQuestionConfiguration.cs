using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CryptoInvestment.Domain.SecurityQuestions;

namespace CryptoInvestment.Infrastucture.SecurityQuestions.Persistance;

public class SecurityQuestionConfiguration : IEntityTypeConfiguration<SecurityQuestion>
{
    public void Configure(EntityTypeBuilder<SecurityQuestion> builder)
    {
        builder.ToTable("customer_qrecovery");

        builder.HasKey(sq => sq.IdSecurityQuestion);

        builder.Property(sq => sq.IdSecurityQuestion)
            .HasColumnName("id_customer_qrecovery")
            .ValueGeneratedOnAdd();

        builder.Property(sq => sq.Question)
            .HasColumnName("question")
            .IsRequired()
            .HasMaxLength(255);
    }
}