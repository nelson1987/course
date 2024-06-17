using Ether.ServiceDefaults.Entities;

namespace Ether.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<Product[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<Product>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<Product>("/api/v1/products", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}
