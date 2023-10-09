using Common.CodeGenPro;
using Common.Enums;
using System.ComponentModel.DataAnnotations;
using static Common.CodeGenPro.GenrateCodeHelper;

namespace Application.Features.GenerateCodes.Commands.Backend;

[Map(typeof(GenerateCodeParam))]
[Description("生成后端代码")]
public class GenerateBackendCodeCommand : IRequest<Result<IEnumerable<string>>>
{
    /// <summary>
    /// 类名: 如 "Domain.Entities.Identity.User";
    /// </summary>
    [Required(ErrorMessage = $"缺少参数FullClassName")]
    public string FullClassName { get; set; }
    /// <summary>
    /// 命名空间: 如 "Application.Features";
    /// </summary>
    [Required(ErrorMessage = $"缺少参数NameSpace")]
    public string? NameSpace { get; set; } = "Application.Features";
    /// <summary>
    /// 保存路径：如 "D:\\Programming\\Net\\Onion\\src\\Application\\Features";
    /// </summary>
    [Required(ErrorMessage = $"缺少参数SavePath")]
    public string SavePath { get; set; }
    /// <summary>
    /// 生成代码类型（枚举）
    /// </summary>
    [Required(ErrorMessage = $"缺少参数Type")]
    public GenerateCodeType Type { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GenerateBackendCodeCommandHandler : IRequestHandler<GenerateBackendCodeCommand, Result<IEnumerable<string>>>
{
    private readonly IMapper _mapper;

    public GenerateBackendCodeCommandHandler(
        IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<IEnumerable<string>>> Handle(GenerateBackendCodeCommand request, CancellationToken cancellationToken)
    {
        var param = _mapper.Map<GenerateCodeParam>(request);
        var filePaths = new List<string>();
        if (param.Type == GenerateCodeType.Caching || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateCachingCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Add || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateAddCommandCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Update || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateUpdateCommandCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Delete || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateDeleteCommandCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.DTOs || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateDTOsCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.EventHandlers || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateEventHandlersCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.GetAll || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateQueriesGetAllCode(
                 GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.GetById || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateQueriesGetByIdCode(
                 GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Pagination || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateQueriesPaginationCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.AdvancedFilter || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsFilterCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.AdvancedPaginationSpec || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsPaginationSpecCode(
                 GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.ByIdSpec || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsByIdSpecCode(
                 GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Controller || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeCQRS.GenerateControllerCode(
                 GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }

        return await Result<IEnumerable<string>>.SuccessAsync(filePaths);
    }
}
