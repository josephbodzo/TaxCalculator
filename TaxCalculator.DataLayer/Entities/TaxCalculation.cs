using System;
using TaxCalculator.Common.Enums;

namespace TaxCalculator.DataLayer.Entities
{
    public class TaxCalculation: BaseEntity<int>
    {
        public string PostalCode { get; set; }
        public decimal AnnualIncome { get; set; }
        public decimal TaxAmount { get; set; }
        public TaxCalculationType CalculationType { get; set; }
        public TaxYear TaxYear { get; set; }
    }
}
