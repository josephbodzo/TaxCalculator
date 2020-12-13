using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.DatabaseContexts.EntityConfigurations
{
    public class PostalCodeCalculationTypeMappingConfiguration  :IEntityTypeConfiguration<PostalCodeCalculationTypeMapping>
    {
        public void Configure(EntityTypeBuilder<PostalCodeCalculationTypeMapping> builder)
        {
            builder.Property(c => c.PostalCode).HasMaxLength(4);
        }
    }
}
