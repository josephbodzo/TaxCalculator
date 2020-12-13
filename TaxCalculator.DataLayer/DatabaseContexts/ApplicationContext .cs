using Microsoft.EntityFrameworkCore;
using TaxCalculator.DataLayer.DatabaseContexts.EntityConfigurations;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.DatabaseContexts
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PostalCodeCalculationTypeMappingConfiguration());
            modelBuilder.ApplyConfiguration(new TaxCalculationConfiguration());
            modelBuilder.ApplyConfiguration(new TaxYearConfiguration());
        }

        public DbSet<FlatRateSetting> FlatRateSettings { get; set; }
        public DbSet<FlatValueSetting> FlatValueSettings { get; set; }
        public DbSet<ProgressiveTaxRateSetting> ProgressiveTaxSettings { get; set; }
        public DbSet<TaxCalculation> TaxCalculations { get; set; }
        public DbSet<TaxYear> TaxYears { get; set; }
        public DbSet<PostalCodeCalculationTypeMapping> PostalCodeCalculationTypeMappings { get; set; }
        
    }
}
