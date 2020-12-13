using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TaxCalculator.Common.Constants;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations.CachedImplementations
{
    public class CachedTaxRateSettingRepository<TTaxRateSetting> : ITaxRateSettingRepository<TTaxRateSetting> where TTaxRateSetting : BaseTaxRateSetting, new()
    {
        private readonly IMemoryCache _cache;
        private readonly TaxRateSettingRepository<TTaxRateSetting> _settingRepository;

        public CachedTaxRateSettingRepository(
            IMemoryCache cache,
            TaxRateSettingRepository<TTaxRateSetting> settingRepository)
        {
            _cache = cache;
            _settingRepository = settingRepository;
        }


        public Task<IList<TTaxRateSetting>> GetByTaxYearAsync(TaxYear taxYear)
        {
            return _cache.GetOrCreateAsync($"{GetType().Name}_{taxYear}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(DefaultValues.CacheTimeInSeconds);
                return _settingRepository.GetByTaxYearAsync(taxYear);
            });
        }
    }
}
