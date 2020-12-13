using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TaxCalculator.Common.Constants;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations.CachedImplementations
{
    public class CachedTaxYearRepository: ITaxYearRepository
    {
        private readonly IMemoryCache _cache;
        private readonly ITaxYearRepository _repository;
        private readonly ApplicationContext _context;

        public CachedTaxYearRepository(
            IMemoryCache cache,
            TaxYearRepository repository,
            ApplicationContext context)
        {
            _cache = cache;
            _repository = repository;
            _context = context;
        }

        public async Task<TaxYear> GetTaxYearAsync(DateTime taxDate)
        {
            var cacheKey = $"{taxDate:dd-MMM-yyyy}";
            var taxYear= await _cache.GetOrCreateAsync($"{GetType().Name}_{cacheKey}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(DefaultValues.CacheTimeInSeconds);
                return _repository.GetTaxYearAsync(taxDate);
            });

            _context.TaxYears.Attach(taxYear);
            return taxYear;
        }
    }
}
