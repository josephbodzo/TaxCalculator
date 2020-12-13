using TaxCalculator.Business.Models;
using TaxCalculator.Common.Responses;
using TaxCalculator.Common.Services;
using TaxCalculator.Common.ValidationRuleEngines;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.ValidationRules.TaxCalculation
{
    public class TaxYearValidationRule : IValidationRule<TaxCalculationRequest, TaxCalculationResponse>
    {
        private readonly ITaxYearRepository _taxYear;
        private readonly IClock _clock;

        public TaxYearValidationRule(ITaxYearRepository taxYear, IClock clock)
        {
            _taxYear = taxYear;
            _clock = clock;
        }

        public OperationResult<TaxCalculationResponse> Validate(TaxCalculationRequest request)
        {
            var operationResult = new OperationResult<TaxCalculationResponse>();
            var taxYear = _taxYear.GetTaxYearAsync(_clock.GetCurrentDateTime()).Result;

            if (taxYear == null)
            {
                operationResult.AddErrorMessage(string.Empty, "Tax year was not found for today's date.");
                return operationResult;
            }

            return operationResult;
        }
    }
}
