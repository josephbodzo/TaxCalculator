using System;
using System.Threading.Tasks;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories
{
    public interface ITaxYearRepository
    {
        Task<TaxYear> GetTaxYearAsync(DateTime taxDate);
    }
}