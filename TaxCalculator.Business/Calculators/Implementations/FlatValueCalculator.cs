using System.Linq;
using TaxCalculator.Common.Responses;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Calculators.Implementations
{
    public class FlatValueCalculator: BaseTaxRateCalculator<FlatValueSetting>
    {
        public FlatValueCalculator(ITaxRateSettingRepository<FlatValueSetting> repository) : base(repository)
        {
        }

        protected override OperationResult<decimal> CalculateTax(decimal annualIncome)
        {
            var result = new OperationResult<decimal>();

            var taxSetting = TaxRateSettings.First();

            if (annualIncome < taxSetting.LowIncomeThreshold)
            {
                result.Response = annualIncome * (taxSetting.LowIncomeTaxRatePerc/100.00M);
            }
            else
            {
                result.Response = taxSetting.HighIncomeTaxAmount;
            }
            return result;
        }

        protected override OperationResult<decimal> Validate()
        {
            var result = new OperationResult<decimal>();

            if (TaxRateSettings?.Count > 1)
            {
                result.AddErrorMessage($"More than 1 Flat Value Tax settings have been found for the year: {TaxYear}");
            }

            return result;
        }
    }
}
