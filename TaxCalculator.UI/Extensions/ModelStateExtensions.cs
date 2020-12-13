using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaxCalculatorApi;

namespace TaxCalculator.UI.Extensions
{
    public static class ModelStateExtensions
    {
        ///TODO: This method can be improved to translate the errors better from the API client
        public static void MapErrors(this ModelStateDictionary modelState, ProblemDetails e)
        {
            foreach (var resultAdditionalProperty in e.AdditionalProperties)
            {
                if (resultAdditionalProperty.Value is JArray array)
                {
                    foreach (var x in array)
                    {
                        modelState.AddModelError(string.Empty, x.ToString());
                    }
                }
                else if (resultAdditionalProperty.Key == "errors")
                {
                    var errors = (JObject)resultAdditionalProperty.Value;
                    foreach (var errorList in errors)
                    {
                        foreach (var error in JsonConvert.DeserializeObject<string[]>(errorList.Value.ToString()))
                        {
                            modelState.AddModelError(string.Empty, error);
                        }
                    }
                }
            }
        }
    }
}
