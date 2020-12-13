using System.Threading.Tasks;
using TaxCalculator.Business.Models;
using TaxCalculator.Common.Responses;

namespace TaxCalculator.Business.Managers
{
    public interface ITaxCalculatorManager
    {
        Task<OperationResult<TaxCalculationResponse>> CalculateTax(TaxCalculationRequest request);
    }
}