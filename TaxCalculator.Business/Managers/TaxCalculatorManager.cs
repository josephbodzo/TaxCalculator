using System.Threading.Tasks;
using TaxCalculator.Business.Factories;
using TaxCalculator.Business.Models;
using TaxCalculator.Common.Responses;
using TaxCalculator.Common.Services;
using TaxCalculator.Common.ValidationRuleEngines;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Managers
{
    public class TaxCalculatorManager : ITaxCalculatorManager
    {
        private readonly ITaxCalculationRepository _calculationRepository;
        private readonly ITaxYearRepository _taxYearRepository;
        private readonly IPostalCodeTaxCalculationMappingRepository _calculationMappingRepository;
        private readonly IValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse> _validationRuleEngine;
        private readonly ITaxCalculatorFactory _taxCalculatorFactory;
        private readonly IClock _clock;

        public TaxCalculatorManager(
            ITaxCalculationRepository calculationRepository,
            ITaxYearRepository taxYearRepository,
            IPostalCodeTaxCalculationMappingRepository calculationMappingRepository, 
            IValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse> validationRuleEngine,
            ITaxCalculatorFactory taxCalculatorFactory,
            IClock clock)
        {
            _calculationRepository = calculationRepository;
            _taxYearRepository = taxYearRepository;
            _calculationMappingRepository = calculationMappingRepository;
            _validationRuleEngine = validationRuleEngine;
            _taxCalculatorFactory = taxCalculatorFactory;
            _clock = clock;
        }

        public async Task<OperationResult<TaxCalculationResponse>> CalculateTax(TaxCalculationRequest request)
        {
            OperationResult<TaxCalculationResponse> validatorResult = _validationRuleEngine.Validate(request);

            if (validatorResult.HasErrors)
            {
                return validatorResult;
            }
            
            var calculationTypeMapping = await _calculationMappingRepository.GetByPostalCodeAsync(request.PostalCode).ConfigureAwait(false);
            var taxCalculator = _taxCalculatorFactory.GetCalculator(calculationTypeMapping.CalculationType);
            var taxYear = await _taxYearRepository.GetTaxYearAsync(_clock.GetCurrentDateTime()).ConfigureAwait(false);
            var taxAmountResult = await taxCalculator.CalculateTaxAsync(taxYear, request.AnnualIncome).ConfigureAwait(false);

            if (taxAmountResult.HasErrors)
            {
                return taxAmountResult.MapErrors<TaxCalculationResponse>();
            }

            var response = new TaxCalculationResponse
            {
                TaxYear = taxYear.ToString(),
                CalculationType = calculationTypeMapping.CalculationType,
                TaxAmount = taxAmountResult.Response
            };

            await SaveCalculation(request, response, taxYear);

            return new OperationResult<TaxCalculationResponse>(response);
        }

        private async Task SaveCalculation(TaxCalculationRequest request, TaxCalculationResponse response, TaxYear taxYear)
        {
            var calculation = new TaxCalculation
            {
                AnnualIncome = request.AnnualIncome,
                TaxAmount = response.TaxAmount,
                CalculationType = response.CalculationType,
                CreatedBy = request.RequestedBy,
                CreationDate = _clock.GetCurrentDateTime(),
                PostalCode = request.PostalCode,
                TaxYear = taxYear
            };

            await _calculationRepository.AddAsync(calculation).ConfigureAwait(false);
        }
    }
}
