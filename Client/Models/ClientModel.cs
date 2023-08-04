using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using API.Models;
using API.Models.Domain;
using API.Models.Exceptions;
using API.Models.Requests;
using API.Models.Responses;
using API.Utils;
using Aspose.Pdf.Operators;
using Base.Functionals;
using Base.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Models;

public class ClientModel : PageModel
{

    private readonly HttpClient _client;
    public String errorMessage;

    public ClientModel()
    {
        _client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        _client.DefaultRequestHeaders.Accept.Add(contentType);
    }

    protected T CallGet<T>(string url)
    {
        HttpResponseMessage response = Asyncs.ApplyAsync(() => _client.GetAsync(url));
        string json = Asyncs.ApplyAsync(() => response.Content.ReadAsStringAsync());
        return DeserializeResponse<T>(json, response);
    }

    private static T DeserializeResponse<T>(string json, HttpResponseMessage response)
    {
        if (json.Length == 0 || typeof(T) == typeof(None))
        {
            return default;
        }

        if (response.IsSuccessStatusCode)
        {
            return Jsons.Deserialize<T>(json);
        }
        try
        {
            IDictionary<String, String[]> validateErrors = Jsons.Deserialize<IDictionary<String, String[]>>(json);
            if (validateErrors != null)
            {
                throw new InputValidationException(validateErrors);
            }
        } catch (JsonException)
        {

        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw ApiException.NotFound(json.Replace("{", "").Replace("}", ""));
        }

        throw ApiException.Failed(json.Replace("{", "").Replace("}", ""));
    }

    protected T CallPost<T>(string url, object? requests = null)
    {
        JsonContent content = JsonContent.Create(requests);
        HttpResponseMessage response = Asyncs.ApplyAsync(() => _client.PostAsync(url, content));
        string json = Asyncs.ApplyAsync(() => response.Content.ReadAsStringAsync());
        return DeserializeResponse<T>(json, response);
    }

    protected T CallPut<T>(string url,  object? requests = null)
    {
        JsonContent content = JsonContent.Create(requests);
        HttpResponseMessage response = Asyncs.ApplyAsync(() => _client.PutAsync(url, content));
        string json = Asyncs.ApplyAsync(() => response.Content.ReadAsStringAsync());
        return DeserializeResponse<T>(json, response);
    }

    protected T CallPatch<T>(string url, object? requests = null)
    {
        JsonContent content = JsonContent.Create(requests);
        HttpResponseMessage response = Asyncs.ApplyAsync(() => _client.PatchAsync(url, content));
        string json = Asyncs.ApplyAsync(() => response.Content.ReadAsStringAsync());
        return DeserializeResponse<T>(json, response);
    }

    protected T CallDelete<T>(string url)
    {
        HttpResponseMessage response = Asyncs.ApplyAsync(() => _client.DeleteAsync(url));
        string json = Asyncs.ApplyAsync(() => response.Content.ReadAsStringAsync());
        return DeserializeResponse<T>(json, response);
    }

    protected IActionResult TakeAction(Supplier<ActionResult> supplier, Func<Exception, ActionResult>? onFailed = null)
    {
        try
        {
            return supplier();
        }
        catch (ApiException ex)
        {
            if (ex.Status == HttpStatusCode.NotFound)
            {
                return ToNotFoundPage();
            }
            TempData["Error"] = ex.Message;
            ViewData["Error"] = ex.Message;
            if (onFailed != null)
            {
                return onFailed(ex);
            }
            return Page();
        }
        catch (InputValidationException ex)
        {
            StoreRequestParams();
            StoreErrorMessages(ex.Errors);
            if (onFailed != null)
            {
                return onFailed(ex);
            }
            return Page();
        }
        catch(Exception e)
        {
            TempData["Error"] = e.Message;
            ViewData["Error"] = "Có lỗi xảy ra, xin hãy thử lại.";
            errorMessage = e.Message;
            if (onFailed != null)
            {
                return onFailed(e);
            }
            return Page();
        }
    }

    private void StoreErrorMessages(IDictionary<string, string[]> errors)
    {
        try
        {
            foreach (var error in errors)
            {
                ViewData["Error" + error.Key] = error.Value[0];
            }
        }
        catch
        {
            //
        }
    }

    protected void StoreRequestParams()
    {
        try
        {
            object?[] requests = GetType().GetProperties().Select(field => field.GetValue(this)).Where(v => v is BaseRequest).ToArray();
            StoreTempData(requests);
        }
        catch
        {
        }
    }

    protected void StoreTempData(params object[] actionArgumentsValues)
    {
        StoreParams.StoreTempData(ViewData, actionArgumentsValues);
    }

    protected bool IsAdmin()
    {
        AccountResponse? user = GetCurrentUser();
        return user?.Role == 1;
    }

    protected bool HasRole(int role)
    {
        string? token = GetAccessToken();
        if (token == null)
        {
            return false;
        }
        ClaimsPrincipal principal = TokenUtils.GetPrincipalFrom(token);

        return principal.IsInRole(role.ToString());
    }

    protected string? GetAccessToken()
    {
        try
        {
            string? token = GetFromCookie<string>("accessToken");
            if (token == null)
            {
                return null;
            }
            ClaimsPrincipal principal = TokenUtils.GetPrincipalFrom(token);
            if (_IsTokenValid(principal))
            {
                return token;
            }
            int? id = Numbers.IntegerOf(principal?.Identity?.Name);
            AuthUser? user = CallPost<AuthUser>($"https://localhost:7176/Account/{id}/RefreshToken?accessToken={token}");

            SaveToCookie("account", user);
            SaveToCookie("accessToken", user.AccessToken);
            SaveToCookie("refreshToken", user.RefreshToken);

            return GetFromCookie<string>("accessToken");
        }
        catch
        {
            return null;
        }
        
    }

    private bool _shouldGenerateNewTokenFor(AuthUser user)
    {
        string? dbRefreshToken = user.RefreshToken;
        string? refreshToken = GetFromCookie<string>("refreshToken");
        return string.Equals(dbRefreshToken, refreshToken) && DateTime.Now <= user.TokenExpireAt;
    }

    protected AccountResponse? GetCurrentUser()
    {
        try
        {
            string? token = GetAccessToken();
            if (token == null)
            {
                return null;
            }
            ClaimsPrincipal principal = TokenUtils.GetPrincipalFrom(token);
            // if (!_IsTokenValid(principal))
            // {
            //     return null;
            // }
            int? id = Numbers.IntegerOf(principal?.Identity?.Name);

            return CallGet<AccountResponse>($"https://localhost:7176/Account/{id}");
        }
        catch (Exception e)
        {
            return null;
        }

    }
    static bool _IsTokenValid(ClaimsPrincipal principal)
    {
        DateTime.TryParse(principal.FindFirst(ClaimTypes.Expiration)?.Value, out DateTime expiration);
        return expiration > DateTime.Now;
    }

    protected Cart GetCart()
    {
        return GetFromSession<Cart>("cart") ?? new();
    }

    protected bool HasLogin()
    {
        return GetCurrentUser() != null;
    }

    protected T? GetFromSession<T>(String key)
    {
        return Sessions.Get<T>(key, HttpContext.Session);
    }

    protected void SaveToSession(String key, object o)
    {
        Sessions.Set(key, o, HttpContext.Session);
    }

    protected void RemoveFromSession(string key)
    {
        HttpContext.Session.Remove(key);
    }

    protected T? GetFromCookie<T>(String key)
    {
        return Sessions.GetFromCookie<T>(key, Request);
    }

    protected void SaveToCookie(String key, object? o)
    {
        Sessions.SaveToCookie(key, o, Response);
    }

    protected void RemoveFromCookie(string key)
    {
        Sessions.RemoveFromCookie(key, Response.Cookies);
    }

    protected IActionResult ToNotFoundPage()
    {
        return RedirectToPage("/NotFound");
    }

    protected IActionResult ToForbiddenPage()
    {
        return RedirectToPage("/InvalidPage");
    }

    protected static ObjectResult Ajax(object value)
    {
        return new ObjectResult(value);
    }
}