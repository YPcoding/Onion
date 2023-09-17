using Application.ExceptionHandlers;

namespace Application.Common.ExceptionHandlers;

public class ForbiddenAccessException : ServerException
{
    public ForbiddenAccessException(string message) : base(message, System.Net.HttpStatusCode.Forbidden) { }
}
