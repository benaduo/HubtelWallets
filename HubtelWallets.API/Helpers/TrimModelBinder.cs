using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HubtelWallets.API.Helpers;

public class TrimModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null) throw new ArgumentNullException(nameof(bindingContext));

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        bindingContext.Result = string.IsNullOrWhiteSpace(value) ?
            ModelBindingResult.Success(null) : ModelBindingResult.Success(value.Trim());

        return Task.CompletedTask;
    }
}