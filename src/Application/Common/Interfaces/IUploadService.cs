namespace Application.Common.Interfaces;

public interface IUploadService : IScopedDependency
{
    Task<string> UploadAsync(UploadRequest request);
}
