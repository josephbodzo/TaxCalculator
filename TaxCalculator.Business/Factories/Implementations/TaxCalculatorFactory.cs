using System;
using TaxCalculator.Business.Calculators;
using TaxCalculator.Business.Calculators.Implementations;
using TaxCalculator.Common.Enums;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.Factories
{
    public  class TaxCalculatorFactory : ITaxCalculatorFactory
    {
        private readonly ITaxRateSettingRepository<FlatRateSetting> _flatRateSettingRepository;
        private readonly ITaxRateSettingRepository<FlatValueSetting> _flatValueSettingRepository;
        private readonly ITaxRateSettingRepository<ProgressiveTaxRateSetting> _progressiveTaxRateSettingRepository;

        public TaxCalculatorFactory(
            ITaxRateSettingRepository<FlatRateSetting> flatRateSettingRepository,
            ITaxRateSettingRepository<FlatValueSetting> flatValueSettingRepository,
            ITaxRateSettingRepository<ProgressiveTaxRateSetting> progressiveTaxRateSettingRepository
            )
        {
            _flatRateSettingRepository = flatRateSettingRepository;
            _flatValueSettingRepository = flatValueSettingRepository;
            _progressiveTaxRateSettingRepository = progressiveTaxRateSettingRepository;
        }

        public  ITaxCalculator GetCalculator(TaxCalculationType calculationType)
        {
            return calculationType switch
            {
                TaxCalculationType.FLAT_RATE => new FlatRateCalculator(_flatRateSettingRepository),
                TaxCalculationType.FLAT_VALUE => new FlatValueCalculator(_flatValueSettingRepository),
                TaxCalculationType.PROGRESSIVE_TAX =>
                    new ProgressiveTaxCalculator(_progressiveTaxRateSettingRepository),
                _ => throw new NotSupportedException("Calculation type is not supported")
            };
        }
    }
}
