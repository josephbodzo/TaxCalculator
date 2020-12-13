using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.DatabaseContexts.EntityConfigurations
{
    public class TaxYearConfiguration :IEntityTypeConfiguration<TaxYear>
    {
        public void Configure(EntityTypeBuilder<TaxYear> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(200);
        }
    }
}
