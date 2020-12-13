using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;

namespace TaxCalculator.DataLayer.Repositories.Implementations
{
    public class TaxYearRepository : ITaxYearRepository
    {
        private readonly ApplicationContext _context;

        public TaxYearRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<TaxYear> GetTaxYearAsync(DateTime taxDate)
        {
            return _context.TaxYears.SingleOrDefaultAsync(f => f.FromDate <= taxDate && f.ToDate >= taxDate);
        }
    }
}
