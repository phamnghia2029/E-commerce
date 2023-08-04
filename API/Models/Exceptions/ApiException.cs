using System.Net;

namespace API.Models.Exceptions;

public class ApiException: Exception
{
    public HttpStatusCode Status { get; set; }
    
    public ApiException() {}

    public ApiException(HttpStatusCode status, String message) : base(message)
    {
        this.Status = status;
    }

    public static ApiException BadRequest(String message)
    {
        return new ApiException(HttpStatusCode.BadRequest, message);
    }
    
    public static ApiException NotFound(String message)
    {
        return new ApiException(HttpStatusCode.NotFound, message);
    }
    
    public static ApiException Unauthorized(String message)
    {
        return new ApiException(HttpStatusCode.Unauthorized, message);
    }
    
    public static ApiException Forbidden(String message)
    {
        return new ApiException(HttpStatusCode.Forbidden, message);
    }
    
    public static ApiException Failed(String message)
    {
        return new ApiException(HttpStatusCode.InternalServerError, message);
    }
}