using System.ComponentModel;

namespace TaxCalculator.Common.Enums
{
    public enum TaxCalculationType
    {
        [Description("Flat Value")]
        FLAT_VALUE = 1,

        [Description("Flat Rate")]
        FLAT_RATE = 2,

        [Description("Progressive Tax")]
        PROGRESSIVE_TAX = 3,
    }
}
