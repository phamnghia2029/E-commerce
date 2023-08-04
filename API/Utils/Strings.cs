using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Utils;

public static class Strings
{
    public static bool IsEmpty(string str)
    {
        return str?.Length == 0;
    }

    public static bool IsNotEmpty(string str)
    {
        return !IsEmpty(str);
    }

    public static bool IsBlank(string str)
    {
        return str == null || str.Trim().Length == 0;
    }

    public static bool IsNotBlank(string str)
    {
        return !IsBlank(str);
    }
        
    public static bool IsEquals(string? first, string? second)
    {
        first ??= "";
        second ??= "";

        return first.Trim().Equals(second.Trim());
    }

    public static bool IsNotEquals(string? first, string? second)
    {
        return !IsEquals(first, second);
    }

    public static string Join(string separator, params string[] words)
    {
        return String.Join(separator, Collections.ListNonNullOf(words));
    }

    public static string Join(string separator, Collection<string> words)
    {
        return String.Join(separator, Collections.ListNonNullOf(words));
    }

    public static string ValueOf(object? obj, string defaultValue = "")
    {
        return obj?.ToString() ?? defaultValue;
    }

    public static string JsonOf(object? obj)
    {
        if (obj == null)
        {
            return "";
        }
        var options = new JsonSerializerOptions()
        {
            MaxDepth = 0,
            IgnoreReadOnlyProperties = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        return JsonSerializer.Serialize(obj, options);
    }

    public static string GenerateRandomStringWithLength(int length)
    {
        Random _random = new();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

}