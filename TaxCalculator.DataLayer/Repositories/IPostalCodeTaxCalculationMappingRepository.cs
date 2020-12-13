using System.Threading.Tasks;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories
{
    public interface IPostalCodeTaxCalculationMappingRepository
    {
        Task<PostalCodeCalculationTypeMapping> GetByPostalCodeAsync(string postalCode);
    }
}