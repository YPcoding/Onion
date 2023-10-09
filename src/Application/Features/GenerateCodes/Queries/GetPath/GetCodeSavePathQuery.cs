using Application.Features.GenerateCodes.DTOs;
using System.Text.RegularExpressions;

namespace Application.Features.GenerateCodes.Queries.GetPath;

/// <summary>
/// 查询代码存放路径
/// </summary>
[Description("查询代码存放路径")]
public class GetCodeSavePathQuery : IRequest<Result<GenerateCodeDto>>
{
}

/// <summary>
/// 处理程序
/// </summary>
public class GetCodeSavePathQueryHandler :
     IRequestHandler<GetCodeSavePathQuery, Result<GenerateCodeDto>>
{

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回全部权限数据</returns>
    public async Task<Result<GenerateCodeDto>> Handle(GetCodeSavePathQuery request, CancellationToken cancellationToken)
    {
        var dto =new GenerateCodeDto();
        string currentDirectory = Directory.GetCurrentDirectory();
        string parentDirectory = Path.GetDirectoryName(currentDirectory)!;
        dto.BackendSavePath =  $"{parentDirectory}\\Application\\Features";
        dto.FrontendSavePath = $"{parentDirectory}\\UI\\src\\views";
        dto.NamespaceName = "Application.Features";

        string entitiesPath = $"{parentDirectory}\\Domain\\Entities"; 

        // 获取文件夹内的所有文件
        string[] files = Directory.GetFiles(entitiesPath, "*.cs", SearchOption.AllDirectories);


        // 遍历每个文件
        foreach (string filePath in files)
        {
            // 读取文件内容
            string fileContent = File.ReadAllText(filePath);
            string namespaceName=string.Empty;
            string className = string.Empty;

            // 使用正则表达式查找命名空间
            Match namespaceMatch = Regex.Match(fileContent, @"namespace\s+([\w.]+)");
            if (namespaceMatch.Success)
            {
                namespaceName = namespaceMatch.Groups[1].Value;
            }

            // 使用正则表达式查找当前类名称
            Match classMatch = Regex.Match(fileContent, @"class\s+([\w.]+)");
            if (classMatch.Success)
            {
                className = classMatch.Groups[1].Value;
            }
            dto.Entities.Add($"{namespaceName}.{className}");
        }

        return await Result<GenerateCodeDto>.SuccessAsync(dto);
    }
}