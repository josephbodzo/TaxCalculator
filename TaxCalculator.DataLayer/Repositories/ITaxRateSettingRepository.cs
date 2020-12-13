using System.Collections.Generic;
using System.Threading.Tasks;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories
{
    public interface ITaxRateSettingRepository<TTaxRateSetting> where TTaxRateSetting : BaseTaxRateSetting, new()
    {
        Task<IList<TTaxRateSetting>> GetByTaxYearAsync(TaxYear taxYear);
    }
}