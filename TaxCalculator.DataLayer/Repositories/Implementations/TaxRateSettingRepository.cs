using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations
{
    public class TaxRateSettingRepository<TTaxSetting> : ITaxRateSettingRepository<TTaxSetting> where TTaxSetting: BaseTaxRateSetting, new()
    {
        private readonly ApplicationContext _context;

        public TaxRateSettingRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IList<TTaxSetting>> GetByTaxYearAsync(TaxYear taxYear)
        {
            return await _context.Set<TTaxSetting>().Where(r => r.TaxYear == taxYear).ToListAsync();
        }
    }
}
