using CryptoInvestment.Domain.Customers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.Customers.Persistance;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer");

        builder.HasKey(c => c.IdCustomer);
        builder.Property(c => c.IdCustomer)
            .HasColumnName("id_customer")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.IdParent)
            .HasColumnName("id_parent");

        builder.Property(c => c.Active)
            .HasColumnName("active")
            .HasDefaultValue(true);

        builder.Property(c => c.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(45);

        builder.Property(c => c.ApellidoPaterno)
            .HasColumnName("apellido_paterno")
            .HasMaxLength(45);

        builder.Property(c => c.ApellidoMaterno)
            .HasColumnName("apellido_materno")
            .HasMaxLength(45);

        builder.Property(c => c.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("date");

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(45);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(70);

        builder.Property(c => c.PasswdLogin)
            .HasColumnName("passwd_login")
            .HasMaxLength(255);

        builder.Property(c => c.EmailValidated)
            .HasColumnName("email_validated")
            .HasDefaultValue(false);

        builder.Property(c => c.ClPais)
            .HasColumnName("cl_pais");

        builder.Property(c => c.IdEstado)
            .HasColumnName("id_estado");

        builder.Property(c => c.City)
            .HasColumnName("city")
            .HasMaxLength(45);

        builder.Property(c => c.PasswdWithdrawal)
            .HasColumnName("passwd_withdrawal")
            .HasMaxLength(255);

        builder.Property(c => c.LockedUp)
            .HasColumnName("locked_up")
            .HasColumnType("datetime");

        builder.Property(c => c.FailedLoginAttempts)
            .HasColumnName("failed_login_attempts")
            .HasDefaultValue(0);

        builder.Property(c => c.AcceptPromoEmail)
            .HasColumnName("accept_promo_email")
            .HasDefaultValue(false);

        builder.Property(c => c.Arrival)
            .HasColumnName("arrival")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.DocsValidated)
            .HasColumnName("docs_validated")
            .HasColumnType("TINYINT")
            .HasDefaultValue(false);

        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.Phone).IsUnique();
        builder.HasIndex(c => new { c.Email, c.PasswdLogin }).IsUnique();
    }
}