using System.Collections.Generic;
using System.Linq;
using TaxCalculator.Common.Responses;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Calculators.Implementations
{
    public class ProgressiveTaxCalculator : BaseTaxRateCalculator<ProgressiveTaxRateSetting>
    {
        public ProgressiveTaxCalculator(ITaxRateSettingRepository<ProgressiveTaxRateSetting> repository) : base(repository)
        {
        }

        protected override OperationResult<decimal> CalculateTax(decimal annualIncome)
        {
            var result = new OperationResult<decimal>();
            /*
             1. Get all settings with fromAmount less than the annual Income. These are all the valid bands
             2. The last band will have ToAmount >annualIncome so you will need to use the annualIncome value
            */
            var taxSettings = TaxRateSettings.Where( t=> t.FromAmount <= annualIncome)
                .OrderBy(t=> t.FromAmount);

            var taxAmounts = new List<decimal>();

            foreach (var taxSetting in taxSettings)
            {
                var maxAmount = (taxSetting.ToAmount ?? annualIncome) < annualIncome ? taxSetting.ToAmount : annualIncome;

                var taxableAmount = maxAmount.Value - taxSetting.FromAmount;

                var taxAmount = taxableAmount * (taxSetting.TaxRatePerc/ 100.00M);
                taxAmounts.Add(taxAmount);
            }

            result.Response = taxAmounts.Sum();
            return result;
        }

    }
}
