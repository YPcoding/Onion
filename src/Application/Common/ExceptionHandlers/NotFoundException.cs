using Application.ExceptionHandlers;

namespace Application.Common.ExceptionHandlers;

public class NotFoundException : ServerException
{


    public NotFoundException(string message)
        : base(message, System.Net.HttpStatusCode.NotFound)
    {
    }
    public NotFoundException(string name, object key)
         : base($"实体 \"{name}\" ({key}) 未找到", System.Net.HttpStatusCode.NotFound)
    {
    }
}
