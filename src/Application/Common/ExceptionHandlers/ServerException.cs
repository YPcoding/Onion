using System.Net;

namespace Application.ExceptionHandlers;

public class ServerException : Exception
{
    public IEnumerable<string> Error { get; }

    public HttpStatusCode StatusCode { get; }

    public ServerException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        Error = new string[] { message };
        StatusCode = statusCode;
    }
}
