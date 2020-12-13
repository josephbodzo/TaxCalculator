namespace TaxCalculator.API.Models
{
    public class TaxCalculationResponseModel
    {
        public string  TaxYear { get; set; }
        public string CalculationType { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
