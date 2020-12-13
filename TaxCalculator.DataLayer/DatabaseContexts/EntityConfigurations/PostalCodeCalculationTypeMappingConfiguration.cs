using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.DatabaseContexts.EntityConfigurations
{
    public class TaxCalculationConfiguration : IEntityTypeConfiguration<TaxCalculation>
    {
        public void Configure(EntityTypeBuilder<TaxCalculation> builder)
        {
            builder.Property(c => c.PostalCode).HasMaxLength(4);
        }
    }
}
