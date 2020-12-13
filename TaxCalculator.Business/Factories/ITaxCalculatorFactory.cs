using TaxCalculator.Business.Calculators;
using TaxCalculator.Common.Enums;

namespace TaxCalculator.Business.Factories
{
    public interface ITaxCalculatorFactory
    {
        ITaxCalculator GetCalculator(TaxCalculationType calculationType);
    }
}