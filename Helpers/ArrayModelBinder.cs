using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Helpers
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Check if the model is enumerable type
            if(!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // Get the value from ValueProvider
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // If the value is null or white space, return success model that is null
            if(string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // Get the Enumerable's type
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            // Get Converter to the Enumerable's type
            var converter = TypeDescriptor.GetConverter(elementType);

            // convert each item to the corresponding type
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            // Make an array of that type, and set it to the model
            var typedArray = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedArray, 0);
            bindingContext.Model = typedArray;

            // set a successful result passing the resulting array
            bindingContext.Result = ModelBindingResult.Success(typedArray);
            return Task.CompletedTask;

        }
    }
}
