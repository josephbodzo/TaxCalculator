using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations
{
    public class PostalCodeTaxCalculationMappingRepository : IPostalCodeTaxCalculationMappingRepository
    {
        private readonly ApplicationContext _context;

        public PostalCodeTaxCalculationMappingRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<PostalCodeCalculationTypeMapping> GetByPostalCodeAsync(string postalCode)
        {
            return _context.PostalCodeCalculationTypeMappings.SingleOrDefaultAsync(f => f.PostalCode == postalCode );
        }
    }
}