using TaxCalculator.Common.Enums;

namespace TaxCalculator.Business.Models
{
    public class TaxCalculationResponse
    {
        public string  TaxYear { get; set; }
        public TaxCalculationType CalculationType { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
