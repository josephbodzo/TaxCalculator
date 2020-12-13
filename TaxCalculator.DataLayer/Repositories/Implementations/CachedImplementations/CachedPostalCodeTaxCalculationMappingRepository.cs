using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TaxCalculator.Common.Constants;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations.CachedImplementations
{
    public class CachedPostalCodeTaxCalculationMappingRepository : IPostalCodeTaxCalculationMappingRepository
    {
        private readonly IMemoryCache _cache;
        private readonly PostalCodeTaxCalculationMappingRepository _calculationMappingRepository;

        public CachedPostalCodeTaxCalculationMappingRepository(
            IMemoryCache cache, 
            PostalCodeTaxCalculationMappingRepository calculationMappingRepository)
        {
            _cache = cache;
            _calculationMappingRepository = calculationMappingRepository;
        }

        public Task<PostalCodeCalculationTypeMapping> GetByPostalCodeAsync(string postalCode)
        {
            return _cache.GetOrCreateAsync($"{GetType().Name}_{postalCode}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(DefaultValues.CacheTimeInSeconds);
                return _calculationMappingRepository.GetByPostalCodeAsync(postalCode);
            });
        }
    }
}
