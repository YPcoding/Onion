using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Common.Models;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace WebAPI.Controllers;

/// <summary>
/// 文件上传管理
/// </summary>
public class UploadController : ApiControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<SystemSettings> _optSystemSettings;

    public UploadController(
        IUploadService uploadService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SystemSettings> optSystemSettings)
    {
        _uploadService = uploadService;
        _httpContextAccessor = httpContextAccessor;
        _optSystemSettings = optSystemSettings;
    }


    /// <summary>
    /// 上传附件
    /// </summary>
    /// <returns>返回附件地址</returns>
    [HttpPost("Enclosure")]
    [AllowAnonymous]

    public async Task<Result<string>> UploadEnclosureAsync(IFormFile file)
    {
        var filestream = file.OpenReadStream();
        var imgStream = new MemoryStream();
        await filestream.CopyToAsync(imgStream);
        imgStream.Position = 0;
        using (var outStream = new MemoryStream())
        {
            using (var image = Image.Load(imgStream))
            {
                image.Mutate(
                   i => i.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(640, 320) }));
                image.Save(outStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
                var filename = file.FileName;
                var fi = new FileInfo(filename);
                var ext = fi.Extension;
                var result = await _uploadService.UploadAsync(new UploadRequest(Guid.NewGuid().ToString() + ext, UploadType.Image, outStream.ToArray()));
                if (result == string.Empty)
                {
                    return await Result<string>.FailureAsync(new string[] { "上传失败" });
                }

                result = result.Replace("\\", "/");
                var host = _optSystemSettings.Value.HostDomainName;

                return await Result<string>.SuccessAsync($"{host}/{result}");
            }
        }
    }

    /// <summary>
    /// 上传附件
    /// </summary>
    /// <returns>返回附件地址</returns>
    [HttpPost("ImageForBase64")]
    [AllowAnonymous]
    public async Task<Result<string>> UploadImageForBase64(RequestBase64 request)
    {
        // 获取 Base64 数据和文件扩展名
        string base64Data = request.Base64.Replace("data:image/png;base64,","");
        string fileExtension = ".jpeg";

        // 将 Base64 数据解码为字节数组
        byte[] imageBytes = Convert.FromBase64String(base64Data);

        // 生成唯一的文件名
        string fileName = Guid.NewGuid().ToString() + fileExtension;

        // 构建文件路径
        string filePath = Path.Combine("Files", "Image", fileName);

        // 保存字节数组为图片文件
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

    /// <summary>
    /// 图片转base64
    /// </summary>
    /// <returns>返回base64字符串</returns>
    [HttpPost("ConvertImageToBase64")]
    [AllowAnonymous]
    public async Task<Result<string>> ConvertImageToBase64(RequestImagePath request)
    {
        var host = _optSystemSettings.Value.HostDomainName;
        request.ImagePath = request.ImagePath.Replace($"{host}/", "");
        byte[] imageBytes = System.IO.File.ReadAllBytes(request.ImagePath);
        string base64String = Convert.ToBase64String(imageBytes);
        return await Result<string>.SuccessAsync($"data:image/jpeg;base64,{base64String}");
    }
}
