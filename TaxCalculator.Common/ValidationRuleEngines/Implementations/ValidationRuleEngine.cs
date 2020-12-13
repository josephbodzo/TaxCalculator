using System.Collections.Generic;
using TaxCalculator.Common.Responses;

namespace TaxCalculator.Common.ValidationRuleEngines.Implementations
{
    public class ValidationRuleEngine<TRequest, TResponse> : IValidationRuleEngine<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidationRule<TRequest, TResponse>> _validationRules;

        public ValidationRuleEngine(IEnumerable<IValidationRule<TRequest, TResponse>> validationRules)
        {
            _validationRules = validationRules;
        }

        public OperationResult<TResponse> Validate(TRequest entity)
        {
            var operationResult = new OperationResult<TResponse>();
            foreach (var validationRule in _validationRules)
            {
                var validationResult = validationRule.Validate(entity);

                if (validationResult.HasErrors)
                {
                    foreach (var keyValuePair in validationResult.GetErrorMessages())
                    {
                        operationResult.AddErrorMessage(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }
            return operationResult;
        }

    }
}
