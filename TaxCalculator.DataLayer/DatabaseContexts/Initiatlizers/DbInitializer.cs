using System;
using System.Linq;
using TaxCalculator.Common.Enums;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.DatabaseContexts.Initiatlizers
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            context.Database.EnsureCreated();

            if (context.TaxYears.Any())
            {
                return;   // DB has been seeded
            }

            var createdBy = Guid.Parse("9353e4af-3686-4f22-9ee5-bc1ea819f676");

            var taxYear = new TaxYear
            {
                CreatedBy = createdBy,
                CreationDate= DateTime.UtcNow,
                FromDate  = new DateTime(2020, 03, 01),
                ToDate =  new DateTime(2021, 02, 28, 23, 59, 59),
                Name = "2021 tax year"
            };

            context.TaxYears.AddRange(taxYear);
            context.SaveChanges();

            var postalCodeMappings = new[]
            {
                new PostalCodeCalculationTypeMapping{ CalculationType = TaxCalculationType.PROGRESSIVE_TAX, PostalCode = "7441", CreationDate = DateTime.UtcNow, CreatedBy = createdBy},
                new PostalCodeCalculationTypeMapping{ CalculationType = TaxCalculationType.FLAT_VALUE, PostalCode = "A100", CreationDate = DateTime.UtcNow, CreatedBy = createdBy},
                new PostalCodeCalculationTypeMapping{ CalculationType = TaxCalculationType.FLAT_RATE, PostalCode = "7000", CreationDate = DateTime.UtcNow, CreatedBy = createdBy},
                new PostalCodeCalculationTypeMapping{ CalculationType = TaxCalculationType.PROGRESSIVE_TAX, PostalCode = "1000", CreationDate = DateTime.UtcNow, CreatedBy = createdBy}
            };

            context.PostalCodeCalculationTypeMappings.AddRange(postalCodeMappings);
            context.SaveChanges();

            var flatRateSettings = new []
            {
                new FlatRateSetting { FlatRatePerc = 17.5M, TaxYear = taxYear, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
            };

            context.FlatRateSettings.AddRange(flatRateSettings);
            context.SaveChanges();

            var flatValueSettings = new[]
            {
                new FlatValueSetting { LowIncomeThreshold = 200000M, LowIncomeTaxRatePerc = 5M, HighIncomeTaxAmount = 10000M, TaxYear = taxYear, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
            };

            context.FlatValueSettings.AddRange(flatValueSettings);
            context.SaveChanges();

            var progressiveTaxSettings = new[]
            {
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 0M, ToAmount = 8350M, TaxRatePerc = 10M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 8351M, ToAmount = 33950M, TaxRatePerc = 15M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 33951M, ToAmount = 82250M, TaxRatePerc = 25M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 82251M, ToAmount = 171550M, TaxRatePerc = 28M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 171551M, ToAmount = 372950M, TaxRatePerc = 33M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
                new ProgressiveTaxRateSetting {TaxYear = taxYear, FromAmount = 372951M, ToAmount = null, TaxRatePerc = 35M, CreatedBy = createdBy, CreationDate = DateTime.UtcNow},
            };

            context.ProgressiveTaxSettings.AddRange(progressiveTaxSettings);
            context.SaveChanges();
        }
    }
}
