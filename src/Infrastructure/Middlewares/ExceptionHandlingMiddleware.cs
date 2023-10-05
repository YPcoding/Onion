using FluentValidation;
using System.Text.Json;

namespace Infrastructure.Middlewares;

internal class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var responseModel = await Result.FailureAsync(new string[] { exception.Message });
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not ServerException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }
            if (!string.IsNullOrEmpty(exception.Message))
            {
                responseModel = await Result.FailureAsync(new string[] { exception.Message });
            }
            switch (exception)
            {
                case ServerException e:
                    response.StatusCode = (int)e.StatusCode;
                    if (e.Error is not null)
                    {
                        responseModel = await Result.FailureAsync(e.Error.ToArray());
                    }
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ValidationException:
                    if (exception.Message is not null)
                    {
                        var validationException = (ValidationException)exception;
                        var errors = validationException.Errors
                            .Select(s => s.ErrorMessage).ToArray();

                        if (!errors.Any())
                        {
                            errors = new string[] { exception.Message };
                        }

                        responseModel = await Result.FailureAsync(errors);
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            _logger.LogError(exception, $"{exception.Message}. 请求失败，状态代码为：{response.StatusCode}");
            JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            };
            await response.WriteAsync(JsonSerializer.Serialize(responseModel, options));
        }
    }
}
