using Common.CodeGenPro;
using Common.Enums;
using System.ComponentModel.DataAnnotations;
using static Common.CodeGenPro.GenrateCodeHelper;

namespace Application.Features.GenerateCodes.Commands.Frontend;

[Map(typeof(GenerateCodeParam))]
[Description("生成前端代码")]
public class GenerateFrontendCodeCommand : IRequest<Result<IEnumerable<string>>>
{
    /// <summary>
    /// 类名: 如 "Domain.Entities.Identity.User";
    /// </summary>
    [Required(ErrorMessage = $"缺少参数FullClassName")]
    public string FullClassName { get; set; }
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
public class GenerateFrontendCodeCommandHandler : IRequestHandler<GenerateFrontendCodeCommand, Result<IEnumerable<string>>>
{
    private readonly IMapper _mapper;

    public GenerateFrontendCodeCommandHandler(
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
    public async Task<Result<IEnumerable<string>>> Handle(GenerateFrontendCodeCommand request, CancellationToken cancellationToken)
    {
        var param = _mapper.Map<GenerateCodeParam>(request);
        var filePaths = new List<string>();

        if (param.Type == GenerateCodeType.Api || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateApiCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Hook || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateHookCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Rule || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateRuleCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Types || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateTypesCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Form || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateFormCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        if (param.Type == GenerateCodeType.Index || param.Type == GenerateCodeType.GenerateAll)
        {
            filePaths.Add(GenerateCodeVue.GenerateIndexCode(
                GetTypeByFullClassName(param.FullClassName),
                param.NameSpace!,
                param.SavePath));
        }
        return await Result<IEnumerable<string>>.SuccessAsync(filePaths);
    }
}
