using System;

namespace TaxCalculator.Business.Models
{
    public class TaxCalculationRequest
    {
        public string PostalCode { get; set; }
        public decimal AnnualIncome { get; set; }
        public Guid RequestedBy { get; set; }
    }
}
