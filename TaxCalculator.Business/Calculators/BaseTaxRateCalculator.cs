using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Common.Responses;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Calculators
{
    public abstract class BaseTaxRateCalculator<TTaxRateSetting>: ITaxCalculator where TTaxRateSetting : BaseTaxRateSetting, new()
    {
        private readonly ITaxRateSettingRepository<TTaxRateSetting> _repository;
        protected IList<TTaxRateSetting> TaxRateSettings;
        protected TaxYear TaxYear;

        protected BaseTaxRateCalculator(ITaxRateSettingRepository<TTaxRateSetting> repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<decimal>> CalculateTaxAsync(TaxYear taxYear, decimal annualIncome)
        {
            TaxYear = taxYear;

            await LoadSettingsAsync();

            var baseValidationResult = BaseValidation();
            if (baseValidationResult.HasErrors)
            {
                return baseValidationResult;
            }

            var validationResult = Validate();
            if (validationResult.HasErrors)
            {
                return validationResult;
            }

            var taxAmountResult =  CalculateTax(annualIncome);
            taxAmountResult.Response = RoundOffAmount(taxAmountResult.Response);
            return taxAmountResult;
        }

        protected abstract OperationResult<decimal> CalculateTax(decimal annualIncome);

        protected virtual OperationResult<decimal> Validate()
        {
            return new OperationResult<decimal>();
        }

        protected virtual async Task LoadSettingsAsync()
        {
            TaxRateSettings = await _repository.GetByTaxYearAsync(TaxYear);
        }
        
        private  OperationResult<decimal> BaseValidation()
        {
            var result = new OperationResult<decimal>();

            if (TaxRateSettings == null || !TaxRateSettings.Any())
            {
                result.AddErrorMessage($"No Tax Rate setting have been found for the tax year: {TaxYear}");
            }

            return result;
        }

        private decimal RoundOffAmount(decimal taxAmount)
        {
            //TODO: Correct rounding method to be determined
            return Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
        }
    }
}
