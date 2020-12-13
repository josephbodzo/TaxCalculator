namespace TaxCalculator.DataLayer.Entities
{
    public class FlatValueSetting: BaseTaxRateSetting
    {
        public decimal LowIncomeThreshold { get; set; }
        public decimal LowIncomeTaxRatePerc { get; set; }
        public decimal HighIncomeTaxAmount { get; set; }
    }
}
