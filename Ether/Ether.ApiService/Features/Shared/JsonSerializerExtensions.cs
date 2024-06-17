using System.Text.Json;

namespace Ether.ApiService.Features.Shared;

public static class JsonSerializerExtensions
{
    public static string ToJson(this object model)
    {
        return JsonSerializer.Serialize(model);
    }
}
