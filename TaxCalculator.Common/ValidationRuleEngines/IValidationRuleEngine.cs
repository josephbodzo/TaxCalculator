using TaxCalculator.Common.Responses;

namespace TaxCalculator.Common.ValidationRuleEngines
{
    public interface IValidationRuleEngine<in TRequest, TResponse >
    {
        OperationResult<TResponse> Validate(TRequest request);
    }
}
