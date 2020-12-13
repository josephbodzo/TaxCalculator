using System.Linq;
using TaxCalculator.Common.Responses;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Calculators.Implementations
{
    public class FlatRateCalculator: BaseTaxRateCalculator<FlatRateSetting>
    {

        public FlatRateCalculator(ITaxRateSettingRepository<FlatRateSetting> repository): base(repository)
        {
        }

        protected override OperationResult<decimal> CalculateTax(decimal annualIncome)
        {
            var result = new OperationResult<decimal> { Response = annualIncome * (TaxRateSettings.First().FlatRatePerc/100.00M)};
            return result;
        }

        protected override OperationResult<decimal> Validate()
        {
            var result = new OperationResult<decimal>();

            if (TaxRateSettings?.Count >1 )
            {
                result.AddErrorMessage($"More than 1 Flat Rate Tax settings have been found for the year: {TaxYear}");
            }

            return result;
        }
    }
}
