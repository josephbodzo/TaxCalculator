using System.Threading.Tasks;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations
{
    public class TaxCalculationRepository : ITaxCalculationRepository
    {
        private readonly ApplicationContext _context;

        public TaxCalculationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaxCalculation calculation)
        {
            _context.TaxCalculations.Add(calculation);
            await _context.SaveChangesAsync();
        }
    }
}
