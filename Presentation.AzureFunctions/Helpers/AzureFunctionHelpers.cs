using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Presentation.AzureFunctions.Helpers;

public static class AzureFunctionHelpers
{
    public static async Task<T> GetModelFromBody<T>(this HttpRequest request)
    {
        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                ArgumentNullException.ThrowIfNull(body);

            var model = JsonConvert.DeserializeObject<T>(body);
            return model != null ? model : default!;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return default!;
        }

    }
}
