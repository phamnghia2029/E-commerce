using System.Net;
using System.Text.Json;
using API.Models.Exceptions;

namespace API.Cores;


public class ExceptionHandler
{
    private readonly RequestDelegate _next;

    public ExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionType = exception.GetType();

        if (exceptionType == typeof(InputValidationException))
        {
            InputValidationException inputValidationException = (InputValidationException) exception;
            IDictionary<string,string[]> apiResponse = inputValidationException.Errors;
            return WriteAsync(context, exception, apiResponse, HttpStatusCode.BadRequest);
        }
        if (exceptionType == typeof(ApiException))
        {
            return WriteAsync(context, exception, exception.Message, ((ApiException)exception).Status);
        }
        return WriteAsync(context, exception, exception.Message, HttpStatusCode.InternalServerError);

    }

    private static Task WriteAsync<T>(HttpContext context, Exception exception, T apiResponse, HttpStatusCode statusCode)
    {
        var exceptionResult = JsonSerializer.Serialize(apiResponse);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        Console.WriteLine(exception.ToString());
        return context.Response.WriteAsync(exceptionResult);
    }
}