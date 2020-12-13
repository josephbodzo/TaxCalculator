using TaxCalculator.Business.Models;
using TaxCalculator.Common.Responses;
using TaxCalculator.Common.ValidationRuleEngines;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.ValidationRules.TaxCalculation
{
    public class PostalCodeValidationRule: IValidationRule<TaxCalculationRequest, TaxCalculationResponse>
    {
        private readonly IPostalCodeTaxCalculationMappingRepository _calculationMappingRepository;

        public PostalCodeValidationRule(IPostalCodeTaxCalculationMappingRepository calculationMappingRepository)
        {
            _calculationMappingRepository = calculationMappingRepository;
        }

        public OperationResult<TaxCalculationResponse> Validate(TaxCalculationRequest request)
        {
            var operationResult = new OperationResult<TaxCalculationResponse>();
            var mapping =  _calculationMappingRepository.GetByPostalCodeAsync(request.PostalCode).Result;

            if (mapping == null)
            {
                operationResult.AddErrorMessage(nameof(request.PostalCode), $"Postal code: {request.PostalCode} was not found.");
                return operationResult;
            }

            return operationResult;
        }
    }
}
