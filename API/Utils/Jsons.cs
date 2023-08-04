using System;
using System.Text.Json;

namespace API.Utils;

public static class Jsons
{
    public static T? Deserialize<T>(String? json)
    {
        if (json == null)
        {
            return default(T);
        }
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<T>(json, options);
    }
}