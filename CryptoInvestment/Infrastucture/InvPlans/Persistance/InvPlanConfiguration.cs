using CryptoInvestment.Domain.InvPlans;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoInvestment.Infrastucture.InvPlans.Persistance;

public class InvPlanConfiguration : IEntityTypeConfiguration<InvPlan>
{
    public void Configure(EntityTypeBuilder<InvPlan> builder)
    {
        builder.ToTable("inv_plans");
        
        builder.HasKey(ip => ip.IdInvPlans);
        
        builder.Property(ip => ip.IdInvPlans)
            .HasColumnName("id_inv_plans")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ip => ip.PlanName)
            .HasColumnName("plan_name")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(ip => ip.MonthsInvested)
            .HasColumnName("months_invested")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(ip => ip.ProfitPercentage)
            .HasColumnName("profit_percentage")
            .HasColumnType("decimal(4,2)")
            .IsRequired(false);

        builder.Property(ip => ip.MinAmount)
            .HasColumnName("min_amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired(false);

        builder.Property(ip => ip.MaxAmount)
            .HasColumnName("max_amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired(false);
    }
}
