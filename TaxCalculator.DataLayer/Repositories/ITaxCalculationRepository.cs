using System.Threading.Tasks;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories
{
    public interface ITaxCalculationRepository
    {
        Task AddAsync(TaxCalculation calculation);
    }
}