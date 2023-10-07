using Application.Common.Configurations;
using Application.Common.Interfaces;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Drawing;

namespace WebAPI.Controllers;

/// <summary>
/// 文件上传管理
/// </summary>
public class UploadController : ApiControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<SystemSettings> _optSystemSettings;
    private readonly ISnowFlakeService _snowflakeService;

    public object SixLabors { get; private set; }

    public UploadController(
        IUploadService uploadService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SystemSettings> optSystemSettings,
        ISnowFlakeService snowflakeService)
    {
        _uploadService = uploadService;
        _httpContextAccessor = httpContextAccessor;
        _optSystemSettings = optSystemSettings;
        _snowflakeService = snowflakeService;
    }

    /// <summary>
    /// 上传附件
    /// </summary>
    /// <returns>返回附件地址</returns>
    [HttpPost("ImageForBase64")]
    [AllowAnonymous]
    public async Task<Result<string>> UploadImageForBase64(RequestBase64 request)
    {
        string base64Data = request.Base64.Replace("data:image/png;base64,", "");
        string fileExtension = GetFileExtension(base64Data);

        byte[] imageBytes = Convert.FromBase64String(base64Data);

        string folderPath = Path.Combine("Files", "Image");
        Directory.CreateDirectory(folderPath);

        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        string filePath = Path.Combine(folderPath, fileName);

        System.IO.File.WriteAllBytes(filePath, imageBytes);

        var result = filePath.Replace("\\", "/");
        var host = _optSystemSettings.Value.HostDomainName;

        return await Result<string>.SuccessAsync($"{host}/{result}");
    }

    public class RequestBase64
    {
        public string Base64 { get; set; }
    }

    public class RequestImagePath
    {
        public string ImagePath { get; set; }
    }

    private string GetFileExtension(string base64)
    {
        if (base64.StartsWith("data:image/png;base64,"))
        {
            return ".png";
        }
        // 添加其他支持的文件类型的处理逻辑
        return ".jpeg"; // 默认扩展名
    }

    /// <summary>
    /// 图片转base64
    /// </summary>
    /// <returns>返回base64字符串</returns>
    [HttpPost("ConvertImageToBase64")]
    public async Task<Result<string>> ConvertImageToBase64(RequestImagePath request)
    {
        using (var httpClient = new HttpClient())
        {
            // 设置超时时间（可选）
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            if (Uri.TryCreate(request.ImagePath, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                byte[] imageBytes = await httpClient.GetByteArrayAsync(uri);
                string base64String = Convert.ToBase64String(imageBytes);
                return await Result<string>.SuccessAsync($"data:image/jpeg;base64,{base64String}");
            }
            else
            {
                return await Result<string>.FailureAsync(new string[] { "无效的图片URL" });
            }
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <returns>返回图片URL</returns>
    [HttpPost("File")]
    public async Task<Result<dynamic>> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return await Result<dynamic>.FailureAsync(new string[] { "文件无效" });
        }

        // 获取文件名
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string filePath = Path.Combine("Files", "Uploads", fileName);
        string fileUrl = $"{_optSystemSettings.Value.HostDomainName}/{filePath}";

        // 创建目录（如果不存在）
        Directory.CreateDirectory(Path.Combine("Files", "Uploads"));

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return await Result<dynamic>.SuccessAsync(new 
        { 
            Id= _snowflakeService.GenerateId(),
            Src = fileUrl,
            FileName = fileName
        });
    }
}
