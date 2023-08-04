using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using API.Entities;

namespace API.Utils;

public class Sessions
{
    public static T? Get<T>(string key, ISession session)
    {
        string? json = session.GetString(key);
        return json == null ? default(T) : JsonSerializer.Deserialize<T>(json);
    }

    public static void Set(string key, Object obj, ISession session)
    {
        session.SetString(key, Strings.JsonOf(obj));
    }

    public static Account? GetCurrentUser(ISession session)
    {
        return  Sessions.Get<Account>("account", session);
    }

    public static void SaveToCookie(String key, object? o, HttpResponse response)
    {
        if (o == null)
        {
            return;
        }
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(14),
            MaxAge = TimeSpan.FromDays(14),
            HttpOnly = true,
            Domain = "localhost"
        };
        response.Cookies.Append(key, Strings.JsonOf(o), cookieOptions);
        backupCookies[key] = Strings.JsonOf(o);
    }

    private static readonly Dictionary<String, String> backupCookies = new();

    public static T? GetFromCookie<T>(string key, HttpRequest request)
    {
        var json = getJson<T>(key, request);
        if (json != null)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        return default;
    }

    private static string? getJson<T>(string key, HttpRequest request)
    {
        try
        {
            string? json = request.Cookies[key];
            if (json != null)
            {
                return json;
            }

            return backupCookies[key];
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static void RemoveFromCookie(string key, IResponseCookies responseCookies)
    {
        responseCookies.Append(key, "", new CookieOptions()
        {
            Expires = DateTime.Now.AddDays(-1)
        });
        try
        {
            backupCookies.Remove(key);
        }
        catch (Exception e)
        {
        }
    }


}