using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Binders
{
    internal sealed class GenericModelBinder<TModelType> : IModelBinder
        where TModelType : class
    {
        private readonly Func<string, (bool success, TModelType? found)> _tryParse;

        public GenericModelBinder(Func<string, (bool success, TModelType? found)> tryParse)
        {
            this._tryParse = tryParse;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string modelKindName = ModelNames.CreatePropertyModelName(prefix: null, propertyName: bindingContext.ModelName);
            string? modelTypeValue = bindingContext.ValueProvider.GetValue(modelKindName)
                                                   .FirstValue;

            if (modelTypeValue == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();

                return Task.CompletedTask;
            }

            (bool success, TModelType? found) = this._tryParse(modelTypeValue);

            if (success && found != null)
            {
                bindingContext.Result = ModelBindingResult.Success(found);

                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Failed();

            return Task.CompletedTask;
        }
    }
}