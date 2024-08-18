using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HubtelWallets.API.Helpers;

public class TrimModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        if (context.Metadata.IsComplexType) return null;

        if (context.Metadata.ModelType == typeof(string)) return new TrimModelBinder();

        return null;
    }
}
