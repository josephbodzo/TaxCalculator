using System.Threading.Tasks;
using TaxCalculator.Common.Responses;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.Business.Calculators
{
    public interface ITaxCalculator
    {
        Task<OperationResult<decimal>> CalculateTaxAsync(TaxYear taxYear, decimal annualIncome);
    }
}
