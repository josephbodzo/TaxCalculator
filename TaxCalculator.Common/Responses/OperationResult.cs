using System.Collections.Generic;
using System.Linq;

namespace TaxCalculator.Common.Responses
{
    public class OperationResult<TResponse>
    {
        private readonly IDictionary<string, IList<string>> _errorMessages;

        public OperationResult()
        {
            _errorMessages= new Dictionary<string, IList<string>>();
        }

        public OperationResult(TResponse response) : this()
        {
            Response = response;
        }

        private OperationResult(IDictionary<string, IList<string>> errorMessages) : this()
        {
            _errorMessages = errorMessages;
        }

        public void AddErrorMessage(string errorKey, string errorMessage)
        {
            AddErrorMessage(errorKey, new List<string>  {errorMessage});
        }

        public void AddErrorMessage(string errorKey, IList<string> errorMessages)
        {
            if (_errorMessages.ContainsKey(errorKey))
            {
                _errorMessages[errorKey].ToList().AddRange(errorMessages);
            }
            else
            {
                _errorMessages.Add(errorKey, errorMessages);
            }
        }

        public void AddErrorMessage(string errorMessage)
        {
            AddErrorMessage(string.Empty, errorMessage);
        }

        public IDictionary<string, IList<string>> GetErrorMessages()
        {
            return _errorMessages;
        }

        public OperationResult<T> MapErrors<T>()
        {
            return new OperationResult<T>(this._errorMessages);
        }

        public bool HasErrors => _errorMessages.Any();

        public TResponse Response { get; set; }

    }
}
