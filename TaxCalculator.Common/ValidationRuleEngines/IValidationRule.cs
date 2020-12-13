using TaxCalculator.Common.Responses;

namespace TaxCalculator.Common.ValidationRuleEngines
{
    public interface IValidationRule<in TRequest, TResponse>
    {
        OperationResult<TResponse> Validate(TRequest entity);
    }
}
