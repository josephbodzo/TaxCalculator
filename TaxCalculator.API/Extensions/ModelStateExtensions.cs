using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TaxCalculator.API.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddErrors(this ModelStateDictionary modelState, IDictionary<string, IList<string>> errors)
        {
            foreach (var (key, errorMessages) in errors)
            {
                foreach (var errorMessage in errorMessages)
                {
                    modelState.AddModelError(key, errorMessage);
                }
            }
        }
    }
}
