using Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

/// <summary>
/// 代码生成器
/// </summary>
namespace Common.CodeGenPro;

public class GenrateCodeHelper
{
    /// <summary>
    /// 判断是否应该生成
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="excludeTypeName"></param>
    /// <returns></returns>
    public static bool IgnoreGenerateToString(string typeName, IEnumerable<string>? excludeTypeName = null)
    {
        if (typeName == null) return false;

        var ignoreTypeNames = new List<string>
        {
             "Id",
             "Deleted",
             "DeletedBy",
             "CreatedBy",
             "LastModified",
             "LastModifiedBy",
             "ConcurrencyStamp",
             "DomainEvents",
             "Created"
        };

        if (excludeTypeName != null && excludeTypeName.Any())
        {
            ignoreTypeNames= ignoreTypeNames.Where(x => !excludeTypeName.Contains(x)).ToList();
        }

        return ignoreTypeNames.Contains(typeName);
    }

    /// <summary>
    /// 将类字段转字符串，用于代码生成
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetCSharpTypeName(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        string typeName = type.Name;

        if (type.IsGenericType)
        {
            typeName = typeName.Split('`')[0] + "<";

            Type[] genericArguments = type.GetGenericArguments();
            for (int i = 0; i < genericArguments.Length; i++)
            {
                typeName += GetCSharpTypeName(genericArguments[i]);
                if (i < genericArguments.Length - 1)
                {
                    typeName += ", ";
                }
            }

            typeName += ">";
        }

        // 将 Int64 转换为 long
        typeName = typeName.Replace("Int64", "long");
        typeName = typeName.Replace("String", "string");
        typeName = typeName.Replace("Object", "object");
        typeName = typeName.Replace("Int32", "int");
        typeName = typeName.Replace("Boolean", "bool");

        // 将 Nullable<T> 转换为 T?
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            typeName = typeName.Replace("Nullable<", "").Replace(">", "?");
            if (!typeName.Contains("?"))
            {
                typeName = $"{typeName}?";
            }
        }

        return typeName;
    }

    /// <summary>
    /// 判断嵌套类中的字段是否为Class
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool GetTypeNameIsClass(Type type)
    {
        return false;
    }

    /// <summary>
    /// 根据字符串获取Type
    /// </summary>
    /// <param name="fullClassName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Type GetTypeByFullClassName(string fullClassName)
    {
        string className = fullClassName;
        Assembly assembly = Assembly.Load(className.Split('.')[0].ToString());
        if (assembly.GetType(className) == null)
        {
            throw new Exception($"请检查名称：{fullClassName}，是否正确");
        }
        return assembly.GetType(className)!;
    }

    /// <summary>
    /// 删除后缀
    /// </summary>
    /// <param name="input">原字符串</param>
    /// <param name="suffixToRemove">需要删除的后缀</param>
    /// <returns>返回移除后缀的字符串</returns>
    public static string RemoveSuffix(string input, string suffixToRemove)
    {
        if (input.EndsWith(suffixToRemove))
        {
            // 使用 Substring 方法删除后缀
            return input.Substring(0, input.Length - suffixToRemove.Length);
        }
        else
        {
            // 如果没有匹配的后缀，返回原始字符串
            return input;
        }
    }

    /// <summary>
    /// 保存代码到文件
    /// </summary>
    /// <param name="text">要保存的文本内容</param>
    /// <param name="savePath">文件路径，包括文件名和扩展名</param>
    public static void SaveTextToFile(string text,string savePath) 
    {
        // 使用 StreamWriter 来写入文本，确保文件编码为 UTF-8
        using (StreamWriter writer = new StreamWriter(savePath, false, Encoding.UTF8))
        {
            writer.Write(text);
        }
    }

    /// <summary>
    /// 获取类字段描述
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTypeDescription(Type type) 
    {
        return type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value?.ToString()!;
    }

    /// <summary>
    /// 生成代码参数
    /// </summary>
    public class GenerateCodeParam
    {
        /// <summary>
        /// 类名: 如 "Domain.Entities.Identity.User";
        /// </summary>
        [Required(ErrorMessage = $"缺少参数FullClassName")]
        public string FullClassName { get; set; }
        /// <summary>
        /// 命名空间: 如 "Application.Features";
        /// </summary>
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
}

/// <summary>
/// 生成CQRS代码
/// </summary>
public class GenerateCodeCQRS
{
    public static string GenerateCachingCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "Caching");
        Directory.CreateDirectory(savePath);
        var filePath = Path.Combine(savePath, $"{typeName}CacheKey.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var body =
$@"using Microsoft.Extensions.Primitives;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
namespace {nameSpace}.{typeName}s.Caching;

public static class {typeName}CacheKey
{{
    public const string GetAllCacheKey = ""all-{typeName}s"";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetByIdCacheKey(long id)
    {{
        return $""Get{typeName}ById,{{id}}"";
    }}

    static {typeName}CacheKey()
    {{
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }}

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {{
        return $""{typeName}sWithPaginationQuery,{{parameters}}"";
    }}

    public static CancellationTokenSource SharedExpiryTokenSource()
    {{
        if (_tokenSource.IsCancellationRequested) _tokenSource = new CancellationTokenSource(RefreshInterval);
        return _tokenSource;
    }}

    public static void Refresh()
    {{
        SharedExpiryTokenSource().Cancel();
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateAddCommandCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string commandFolder = Path.Combine(savePath, $"{typeName}s", "Commands", "Add");
        Directory.CreateDirectory(commandFolder);
        string filePath = Path.Combine(commandFolder, $"Add{typeName}Command.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using System.ComponentModel.DataAnnotations;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using {nameSpace}.{typeName}s.Caching;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace {nameSpace}.{typeName}s.Commands.Add;

/// <summary>
/// 添加{desc}
/// </summary>
[Map(typeof({typeName}))]
public class Add{typeName}Command : IRequest<Result<long>>
{{";
        var body = "";
        PropertyInfo[] properties = type.GetProperties();

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            string propertyTypeName = GenrateCodeHelper.GetCSharpTypeName(property.PropertyType);
            string propertyName = property.Name;

            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

            if (propertyName.Contains("Id")) continue;
            if (GenrateCodeHelper.IgnoreGenerateToString(propertyName)) continue;

            body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {propertyName} {{ get; set; }}";

        }
        body +=
$@"
}}";

        var footer =
$@"
/// <summary>
/// 处理程序
/// </summary>
public class Add{typeName}CommandHandler : IRequestHandler<Add{typeName}Command, Result<long>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Add{typeName}CommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {{
        _context = context;
        _mapper = mapper;
    }}

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name=""request"">请求参数</param>
    /// <param name=""cancellationToken"">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(Add{typeName}Command request, CancellationToken cancellationToken)
    {{
        var {typeName.ToLower()} = _mapper.Map<{typeName}>(request);
        //{typeName.ToLower()}.AddDomainEvent(new CreatedEvent<{typeName}>({typeName.ToLower()}));
        await _context.{typeName}s.AddAsync({typeName.ToLower()});
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync({typeName.ToLower()}.Id, isSuccess, new string[] {{ ""操作失败"" }});
    }}
}}";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateUpdateCommandCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "Commands", "Update");
        Directory.CreateDirectory(savePath);
        var filePath = Path.Combine(savePath, $"Update{typeName}Command.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using {nameSpace}.{typeName}s.Caching;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{typeName}s.Commands.Update;


/// <summary>
/// 修改{desc}
/// </summary>
[Map(typeof({typeName}))]
public class Update{typeName}Command : IRequest<Result<long>>
{{
";
        var body = "";
        PropertyInfo[] properties = type.GetProperties();

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            string propertyTypeName = GenrateCodeHelper.GetCSharpTypeName(property.PropertyType);
            string propertyName = property.Name;

            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

            if (GenrateCodeHelper.IgnoreGenerateToString(propertyName, new string[] { "Id", "ConcurrencyStamp" })) continue;

            if (propertyName == "Id")
            {
                body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {typeName}{propertyName} {{ get; set; }}";

            }
            else
            {
                body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {propertyName} {{ get; set; }}";

            }
        }
        body +=
$@"
}}";

        var footer = $@"

/// <summary>
/// 处理程序
/// </summary>
public class Update{typeName}CommandHandler : IRequestHandler<Update{typeName}Command, Result<long>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Update{typeName}CommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {{
        _context = context;
        _mapper = mapper;
    }}

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name=""request"">请求参数</param>
    /// <param name=""cancellationToken"">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(Update{typeName}Command request, CancellationToken cancellationToken)
    {{
        var {typeName.ToLower()} = await _context.{typeName}s
           .SingleOrDefaultAsync(x => x.Id == request.{typeName}Id, cancellationToken)
           ?? throw new NotFoundException($""数据【{{request.{typeName}Id}}】未找到"");

        {typeName.ToLower()} = _mapper.Map(request, {typeName.ToLower()});
        //{typeName.ToLower()}.AddDomainEvent(new UpdatedEvent<{typeName}>({typeName.ToLower()}));
        _context.{typeName}s.Update({typeName.ToLower()});
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync({typeName.ToLower()}.Id, isSuccess, new string[] {{ ""操作失败"" }});
    }}
}}
";


        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateDeleteCommandCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "Commands", "Delete");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"Delete{typeName}Command.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using {nameSpace}.{typeName}s.Caching;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using Domain.Entities;

namespace {nameSpace}.{typeName}s.Commands.Delete;

/// <summary>
/// 删除{desc}
/// </summary>
public class Delete{typeName}Command : IRequest<Result<bool>>
{{
";
        var body =
$@"  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description(""唯一标识"")]
        public List<long> {typeName}Ids {{ get; set; }}";
        body +=
$@"
}}";

        var footer = $@"

/// <summary>
/// 处理程序
/// </summary>
public class Delete{typeName}CommandHandler : IRequestHandler<Delete{typeName}Command, Result<bool>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Delete{typeName}CommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {{
        _context = context;
        _mapper = mapper;
    }}

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name=""request"">请求参数</param>
    /// <param name=""cancellationToken"">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(Delete{typeName}Command request, CancellationToken cancellationToken)
    {{
        var {typeName.ToLower()}sToDelete = await _context.{typeName}s
            .Where(x => request.{typeName}Ids.Contains(x.Id))
            .ToListAsync();

        if ({typeName.ToLower()}sToDelete.Any())
        {{
            _context.{typeName}s.RemoveRange({typeName.ToLower()}sToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {{""操作失败""}});
        }}

        return await Result<bool>.FailureAsync(new string[] {{ ""没有找到需要删除的数据"" }});
    }}
}}";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateDTOsCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "DTOs");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"{typeName}Dto.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using Domain.Entities;
using Domain.Enums;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{typeName}s.DTOs
{{
    [Map(typeof({typeName}))]
    public class {typeName}Dto
    {{";

        var body =$@"";
        PropertyInfo[] properties = type.GetProperties();
        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            string propertyTypeName = GenrateCodeHelper.GetCSharpTypeName(property.PropertyType);
            string propertyName = property.Name;

            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (GenrateCodeHelper.IgnoreGenerateToString(propertyName, new string[] { "Id", "ConcurrencyStamp" })) continue;

            if (propertyName == "Id") 
            {
                body +=
$@"     

        /// <summary>
        /// {description}
        /// </summary>
        public {propertyTypeName} {typeName}Id 
        {{
            get 
            {{
                return Id;
            }}
        }}";
            }

            body +=
 $@"    

        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {property.Name} {{ get; set; }}";

        }

        var footer =
$@"
    }}
}}";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;

    }

    public static string GenerateEventHandlersCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "EventHandlers");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"{typeName}CreatedEventHandler.cs");

        var body =
$@"using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
namespace {nameSpace}.{typeName}s.EventHandlers;

public class {typeName}CreatedEventHandler : INotificationHandler<CreatedEvent<{typeName}>>
{{
    private readonly ILogger<{typeName}CreatedEventHandler> _logger;

    public {typeName}CreatedEventHandler(
        ILogger<{typeName}CreatedEventHandler> logger
    )
    {{
        _logger = logger;
    }}

    public Task Handle(CreatedEvent<{typeName}> notification, CancellationToken cancellationToken)
    {{
        return Task.CompletedTask;
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateQueriesGetAllCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "Queries", "GetAll");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"GetAll{typeName}Query.cs");

        var body =
$@"using {nameSpace}.{typeName}s.Caching;
using {nameSpace}.{typeName}s.DTOs;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using AutoMapper.QueryableExtensions;

namespace {nameSpace}.{typeName}s.Queries.GetAll;

public class GetAll{typeName}sQuery : IRequest<Result<IEnumerable<{typeName}Dto>>>
{{
}}

public class GetAll{typeName}sQueryHandler :
    IRequestHandler<GetAll{typeName}sQuery, Result<IEnumerable<{typeName}Dto>>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAll{typeName}sQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {{
        _context = context;
        _mapper = mapper;
    }}

    public async Task<Result<IEnumerable<{typeName}Dto>>> Handle(GetAll{typeName}sQuery request, CancellationToken cancellationToken)
    {{
        var data = await _context.{typeName}s
            .ProjectTo<{typeName}Dto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<{typeName}Dto>>.SuccessAsync(data);
    }}
}}

";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateQueriesGetByIdCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;

        savePath = Path.Combine(savePath, $"{typeName}s", "Queries", "GetById");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"Get{typeName}QueryById.cs");

        var body =
$@"using Application.Common.Extensions;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using {nameSpace}.{typeName}s.Caching;
using {nameSpace}.{typeName}s.DTOs;
using {nameSpace}.{typeName}s.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{typeName}s.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
public class Get{typeName}QueryById : IRequest<Result<{typeName}Dto>>
{{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = ""唯一标识必填的"")]
    public long {typeName}Id {{ get; set; }}
}}

/// <summary>
/// 处理程序
/// </summary>
public class Get{typeName}ByIdQueryHandler :IRequestHandler<Get{typeName}QueryById, Result<{typeName}Dto>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Get{typeName}ByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {{
        _context = context;
        _mapper = mapper;
    }}

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name=""request"">请求参数</param>
    /// <param name=""cancellationToken"">取消标记</param>
    /// <returns>返回查询的一条数据</returns>
    /// <exception cref=""NotFoundException"">未找到数据移除处理</exception>
    public async Task<Result<{typeName}Dto>> Handle(Get{typeName}QueryById request, CancellationToken cancellationToken)
    {{
        var {typeName.ToLower()} = await _context.{typeName}s.ApplySpecification(new {typeName}ByIdSpec(request.{typeName}Id))
                     .ProjectTo<{typeName}Dto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($""唯一标识: [{{request.{typeName}Id}}] 未找到"");
        return await Result<{typeName}Dto>.SuccessAsync({typeName.ToLower()});
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateQueriesPaginationCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = Path.Combine(savePath, $"{typeName}s", "Queries", "Pagination");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"{typeName}sWithPaginationQuery.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var body =
$@"using Application.Common.Extensions;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using {nameSpace}.{typeName}s.Caching;
using {nameSpace}.{typeName}s.DTOs;
using {nameSpace}.{typeName}s.Specifications;

namespace {nameSpace}.{typeName}s.Queries.Pagination;

/// <summary>
/// {desc}分页查询
/// </summary>
public class {typeName}sWithPaginationQuery : {typeName}AdvancedFilter, IRequest<Result<PaginatedData<{typeName}Dto>>>
{{
    [JsonIgnore]
    public {typeName}AdvancedPaginationSpec Specification => new {typeName}AdvancedPaginationSpec(this);
}}

/// <summary>
/// 处理程序
/// </summary>
public class {typeName}sWithPaginationQueryHandler :
    IRequestHandler<{typeName}sWithPaginationQuery, Result<PaginatedData<{typeName}Dto>>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public {typeName}sWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {{
        _context = context;
        _mapper = mapper;
    }}

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name=""request"">请求参数</param>
    /// <param name=""cancellationToken"">取消标记</param>
    /// <returns>返回{desc}分页数据</returns>
    public async Task<Result<PaginatedData<{typeName}Dto>>> Handle(
        {typeName}sWithPaginationQuery request,
        CancellationToken cancellationToken)
    {{
        var {typeName.ToLower()}s = await _context.{typeName}s
            .OrderBy($""{{request.OrderBy}} {{request.SortDirection}}"")
            .ProjectToPaginatedDataAsync<{typeName}, {typeName}Dto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<{typeName}Dto>>.SuccessAsync({typeName.ToLower()}s);
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateSpecificationsFilterCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = $"{savePath}\\{typeName}s\\Specifications";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{typeName}AdvancedFilter.cs";
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
namespace {nameSpace}.{typeName}s.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class {typeName}AdvancedFilter : PaginationFilter
{{";

        var body = $@"";

        PropertyInfo[] properties = type.GetProperties();
        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (ignoreFields.Contains(property.Name)) continue;
            if (property.Name.Contains("Id")) continue;
            if (propertyTypeName == typeName) continue;

            if (propertyTypeName == "String")
            {
                propertyTypeName = "string";
                body +=
$@"       
    /// <summary>
    /// {description}
    /// </summary>
    [Description(""{description}"")]
    public {propertyTypeName}? {property.Name} {{ get; set; }}
";
            }
            if (propertyTypeName == "Boolean")
            {
                propertyTypeName = "bool";
                body +=
$@"       
    /// <summary>
    /// {description}
    /// </summary>
    [Description(""{description}"")]
    public {propertyTypeName}? {property.Name} {{ get; set; }}
";
            }
        }

        var footer =
$@"}}";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateSpecificationsPaginationSpecCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        savePath = $"{savePath}\\{typeName}s\\Specifications";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{typeName}AdvancedPaginationSpec.cs";
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"using Ardalis.Specification;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};


namespace {nameSpace}.{typeName}s.Specifications;

public class {typeName}AdvancedPaginationSpec : Specification<{typeName}>
{{
    public {typeName}AdvancedPaginationSpec({typeName}AdvancedFilter filter)
    {{
        Query";

        var body = $@"";

        PropertyInfo[] properties = type.GetProperties();
        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (ignoreFields.Contains(property.Name)) continue;
            if (property.Name.Contains("Id")) continue;
            if (propertyTypeName == typeName) continue;

            if (propertyTypeName == "String")
            {
                propertyTypeName = "string";
                body +=
$@"     
            .Where(x => x.{property.Name} == filter.{property.Name}, !filter.{property.Name}.IsNullOrEmpty())
";
            }
            if (propertyTypeName == "Boolean")
            {
                propertyTypeName = "bool";
                body +=
$@"     
            .Where(x => x.{property.Name}  == filter.{property.Name}, filter.{property.Name}.HasValue)
";
            }
        }

        var footer =
$@";    }}
}}";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateSpecificationsByIdSpecCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        // 使用 Path.Combine 构建目录路径
        savePath = Path.Combine(savePath, $"{typeName}s", "Specifications");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, $"{typeName}ByIdSpec.cs");

        var body =
$@"using Ardalis.Specification;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};

namespace {nameSpace}.{typeName}s.Specifications;

public class {typeName}ByIdSpec : Specification<{typeName}>
{{
    public {typeName}ByIdSpec(long id)
    {{
        Query.Where(q => q.Id == id);
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }

    public static string GenerateControllerCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string fullPath = savePath;
        string targetSubstring = "Onion\\src";
        // 找到目标子字符串的索引
        int index = fullPath.IndexOf(targetSubstring);
        if (index != -1)
        {
            // 截取目标子字符串之前的部分
            fullPath = fullPath.Substring(0, index);
        }

        string directoryPath = Path.Combine(fullPath, targetSubstring, "WebAPI\\Controllers");
        Directory.CreateDirectory(directoryPath);
        string filePath = Path.Combine(directoryPath, $"{typeName}Controller.cs");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var body =
$@"using {nameSpace}.{typeName}s.DTOs;
using {nameSpace}.{typeName}s.Commands.Add;
using {nameSpace}.{typeName}s.Commands.Delete;
using {nameSpace}.{typeName}s.Commands.Update;
using {nameSpace}.{typeName}s.Queries.Pagination;
using {GenrateCodeHelper.RemoveSuffix($"{type.FullName}", $".{typeName}")};
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// {desc}
/// </summary>
public class {typeName}Controller : ApiControllerBase
{{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost(""PaginationQuery"")]

    public async Task<Result<PaginatedData<{typeName}Dto>>> PaginationQuery({typeName}sWithPaginationQuery query)
    {{
        return await Mediator.Send(query);
    }}

    /// <summary>
    /// 创建{desc}
    /// </summary>
    /// <returns></returns>
    [HttpPost(""Add"")]

    public async Task<Result<long>> Add(Add{typeName}Command command)
    {{
        return await Mediator.Send(command);
    }}

    /// <summary>
    /// 修改{desc}
    /// </summary>
    /// <returns></returns>
    [HttpPut(""Update"")]
    public async Task<Result<long>> Update(Update{typeName}Command command)
    {{
        return await Mediator.Send(command);
    }}

    /// <summary>
    /// 删除{desc}
    /// </summary>
    /// <returns></returns>
    [HttpDelete(""Delete"")]
    public async Task<Result<bool>> Delete(Delete{typeName}Command command)
    {{
        return await Mediator.Send(command);
    }}
}}
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
}

/// <summary>
/// 生成前端Vue代码
/// </summary>
public class GenerateCodeVue
{
    public static string FirstCharToLowerCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        char[] chars = input.ToCharArray();
        chars[0] = char.ToLower(chars[0]);
        return new string(chars);
    }
    public static Type GetTypeByFullClassName(string fullClassName = "Domain.Entities.Identity.User")
    {
        string className = fullClassName;
        Assembly assembly = Assembly.Load(className.Split('.')[0].ToString());
        return assembly.GetType(className)!;
    }
    public static string GenerateApiCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string[] folders = savePath.Split(Path.DirectorySeparatorChar);
        string lastFolder = folders[folders.Length - 1];

        if (savePath.Contains("Onion\\src"))
        {
            string fullPath = savePath;
            string targetSubstring = "Onion\\src";
            // 找到目标子字符串的索引
            int index = fullPath.IndexOf(targetSubstring);
            if (index != -1)
            {
                // 截取目标子字符串之前的部分
                fullPath = fullPath.Substring(0, index);
            }
            savePath = $"{fullPath}\\{targetSubstring}\\UI\\src\\api\\{lastFolder}";
        }
        else
        {
            savePath = $"{savePath}\\api\\";
        }
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\{typeName.ToLower()}.ts";
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var body =
$@"
import {{ http }} from ""@/utils/http"";

type Result = {{
  succeeded: boolean;
  data?: object;
  errors: Array<string>;
  errorMessage: string;
  code: number;
}};

type ResultTable = {{
  succeeded: boolean;
  errors: Array<any>;
  errorMessage: string;
  code: number;
  data?: {{
    /** 总条目数 */
    totalItems?: number;
    /** 当前页数 */
    currentPage?: number;
    /** 每页大小 */
    pageSize?: number;
    /** 总页数 */
    totalPages?: number;
    /** 有上一页 */
    hasPreviousPage?: boolean;
    /** 有下一页 */
    hasNextPage?: boolean;
    /** 列表数据 */
    items: Array<any>;
  }};
}};

/** 分页查询 */
export const get{typeName}List = (data?: object) => {{
  return http.request<ResultTable>(""post"", ""/api/{typeName}/PaginationQuery"", {{
    data
  }});
}};

/** 新增 */
export const add{typeName} = (data?: object) => {{
  return http.request<Result>(""post"", ""/api/{typeName}/Add"", {{
    data
  }});
}};

/** 修改 */
export const update{typeName} = (data?: object) => {{
  return http.request<Result>(""put"", ""/api/{typeName}/Update"", {{
    data
  }});
}};

/** 批量删除 */
export const onbatchDelete{typeName} = (data?: object) => {{
  return http.request<Result>(""delete"", ""/api/{typeName}/Delete"", {{
    data
  }});
}};
";
        var code = $"{body}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
    public static string GenerateHookCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string[] folders = savePath.Split(Path.DirectorySeparatorChar);
        string lastFolder = folders[folders.Length - 1];

        string fullPath = savePath;
        // 构建目录路径
        savePath = Path.Combine(fullPath, typeName.ToLower(), "utils");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, "hook.tsx");
        // 构建文件路径
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var import = "";//引入组件
        var hookFunction = "";//Hook功能
        var constDefineHeader = "";//常量头部
        var constDefineBody = "";//常量主体
        var constDefineFooter = "";//常量尾部
        var onSearch = "";//分页查询
        var resetFormSearch = "";//重置查询
        var sizeChange = "";  //改变页码大小
        var currentChange = "";//跳到指定页码
        var columnsHeader = "";//数据列表行头部
        var columnsBody = "";//数据列表行主体
        var columnsFooter = "";//数据列表行尾部
        var onMounted = "";//生命周期钩子函数
        var openDialogHeader = "";//新增或修改
        var openDialogBody = "";//新增或修改
        var openDialogFooter = "";//新增或修改
        var delete = "";//删除
        var onbatchDel = "";//批量删除
        var selectionChangeNum = "";//选择行
        var selectionCancel = "";//取消选择
        var defineFunction = "";//自定义函数
        var buttonClass = "";//按钮样式
        var returnFunction = "";//返回功能

        //引入组件
        import =
$@"
//引入组件
import dayjs from ""dayjs"";
import {{
  get{typeName}List,
  add{typeName},
  update{typeName},
  onbatchDelete{typeName}
}} from ""@/api/{lastFolder}/{typeName.ToLower()}"";
import {{ type PaginationProps }} from ""@pureadmin/table"";
import {{ usePublicHooks }} from ""@/views/system/hooks"";
import {{ message }} from ""@/utils/message"";
import {{ getKeyList }} from ""@pureadmin/utils"";
import type {{ FormItemProps }} from ""../utils/types"";
import {{ addDialog }} from ""@/components/ReDialog"";
import editForm from ""../form/index.vue"";
import {{ type Ref, h, ref, toRaw, computed, reactive, onMounted }} from ""vue"";
import {{ getAuths }} from ""@/router/utils"";";

        //Hook功能
        hookFunction =
$@"
//功能
export function use{typeName}(tableRef: Ref, treeRef: Ref) {{
";
        constDefineHeader =
$@"  //常量
  const form = reactive({{
";
        constDefineBody = $@"";
        PropertyInfo[] properties = type.GetProperties();

        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            bool isBreak = false;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    propertyTypeName = @"""""";
                    break;
                case "Boolean":
                    propertyTypeName = @"null";
                    break;
                case "Int32":
                    propertyTypeName = @"0";
                    break;
                case "Int64":
                    propertyTypeName = @"0";
                    break;
                case "DateTime":
                    isBreak = true;
                    break;
                case "ICollection`1":
                    isBreak = true;
                    break;
                case "Nullable`1":
                    isBreak = true;
                    break;
                case "IReadOnlyCollection`1":
                    isBreak = true;
                    break;
                default:
                    isBreak = true;
                    break;
            }
            if (isBreak) continue;
            constDefineBody +=
$@"
    {FirstCharToLowerCase(property.Name)}: {propertyTypeName},
";
        }
        constDefineFooter =
$@"
    orderBy: ""Id"",
    sortDirection: ""Descending"",
    pageNumber: 1,
    pageSize: 10
  }});
  const formRef = ref();
  const dataList = ref([]);
  const loading = ref(true);
  const switchLoadMap = ref({{}});
  const selectedNum = ref(0);
  const {{ switchStyle }} = usePublicHooks();
  const pagination = reactive<PaginationProps>({{
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  }});
";
        onSearch =
$@"  
  //分页查询
  async function onSearch() {{
    const {{ data }} = await get{typeName}List(toRaw(form));
    dataList.value = data.items;
    pagination.total = data.totalItems;
    pagination.pageSize = data.pageSize;
    pagination.currentPage = data.currentPage;

    setTimeout(() => {{
      loading.value = false;
    }}, 500);
  }}
";
        resetFormSearch =
$@"
  //重置查询
  const resetForm = formEl => {{
    if (!formEl) return;
    formEl.resetFields();
    treeRef.value.onTreeReset();
    onSearch();
  }};
";
        sizeChange =
$@"
  //改变页码大小
  function handleSizeChange(val: number) {{
    form.pageSize = val;
    onSearch();
  }}
";
        currentChange =
$@"  
  //跳到指定页码
  function handleCurrentChange(val: number) {{
    form.pageNumber = val;
    onSearch();
  }}";
        columnsHeader =
$@"
  //数据列表行
  const columns: TableColumnList = [
    {{
      label: ""勾选列"", // 如果需要表格多选，此处label必须设置
      type: ""selection"",
      fixed: ""left"",
      reserveSelection: true // 数据刷新后保留选项
    }},
    {{
      label: ""编号"",
      prop: ""id"", // 假设 id 包含19位的雪花ID
      width: 102,
      formatter: row => {{
        if (typeof row.id === ""string"" && row.id.length === 19) {{
          // 如果 id 是字符串且长度为19，截取后10位并显示
          return row.id.substr(9); // 从第9位开始截取后10位
        }} else {{
          // 如果不符合要求，直接显示原始值
          return row.id;
        }}
      }}
    }},
";
        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            bool isBreak = false;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    columnsBody +=
$@"
    {{
      label: ""{description}"",
      prop: ""{FirstCharToLowerCase(property.Name)}"",
      minWidth: 100
    }},
";
                    break;
                case "Boolean":
                    columnsBody +=
$@"
    {{
      label: ""{description}"",
      prop: ""{FirstCharToLowerCase(property.Name)}"",
      minWidth: 100,
      cellRenderer: scope => (
        <el-switch
          size={{scope.props.size === ""small"" ? ""small"" : ""default""}}
          loading={{switchLoadMap.value[scope.index]?.loading}}
          v-model={{scope.row.{FirstCharToLowerCase(property.Name)}}}
          active-value={{true}}
          inactive-value={{false}}
          active-text=""是""
          inactive-text=""否""
          inline-prompt
          style={{switchStyle.value}}
          onChange={{() => handle{property.Name}OnChange(scope.row)}}
        />
      )
    }},
";
                    defineFunction +=
$@"
  //数据列表自定义生成函数
  async function handle{property.Name}OnChange(row) {{
    message(`功能未实现`, {{ type: ""success"" }});
  }}

";

                    break;
                case "Int32":
                    columnsBody +=
$@"
    {{
      label: ""{description}"",
      minWidth: 100,
      prop: ""{FirstCharToLowerCase(property.Name)}"",
    }},
";
                    break;
                case "Int64":
                    if (property.Name == "Id")
                    {
                        columnsBody +=
$@"
    {{
      label: ""编号"",
      prop: ""id"", // 假设 id 包含19位的雪花ID
      width: 102,
      formatter: row => {{
        if (typeof row.id === ""string"" && row.id.length === 19) {{
          // 如果 id 是字符串且长度为19，截取后10位并显示
          return row.id.substr(9); // 从第9位开始截取后10位
        }} else {{
          // 如果不符合要求，直接显示原始值
          return row.id;
        }}
      }}
    }},
";
                    }
                    else
                    {
                        columnsBody +=
$@"
    {{
      label: ""{description}"",
      prop: ""{FirstCharToLowerCase(property.Name)}"",
      minWidth: 100
    }},
";
                    }
                    break;
                case "DateTime":
                    columnsBody +=
$@"
    {{
      label: ""{description}"",
      minWidth: 100,
      prop: ""{FirstCharToLowerCase(property.Name)}"",
      formatter: ({{ {FirstCharToLowerCase(property.Name)} }}) => dayjs({FirstCharToLowerCase(property.Name)}).format(""YYYY-MM-DD HH:mm:ss"")
    }},
";
                    break;
                default:
                    break;
            }

        }

        columnsFooter =
$@"    
    {{
      label: ""操作"",
      fixed: ""right"",
      width: 180,
      slot: ""operation"",
      hide: () => {{
        // 判断权限是否可以显示操作栏
        const auths = [""api:{typeName.ToLower()}:update"", ""api:{typeName.ToLower()}:delete""];
        return !usePublicHooks().hasAuthIntersection(getAuths(), auths);
      }}
    }}
  ];";
        onMounted +=
$@"
  //生命周期钩子函数
  onMounted(async () => {{
    onSearch();
  }});
";
        openDialogHeader +=
$@"
  /** 新增或修改 */
  async function openDialog(title = ""新增"", row?: FormItemProps) {{
    addDialog({{
      title: `${{title}}{desc}`,
      props: {{
        formInline: {{
          title,
          {FirstCharToLowerCase(typeName)}Id: row?.{FirstCharToLowerCase(typeName)}Id ?? """",
";
        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    openDialogBody +=
$@"
          {FirstCharToLowerCase(property.Name)}: row?.{FirstCharToLowerCase(property.Name)} ?? """",
";
                    break;
                case "Boolean":
                    openDialogBody +=
$@"
          {FirstCharToLowerCase(property.Name)}: row?.{FirstCharToLowerCase(property.Name)} ?? true,
";
                    break;
                case "Int32":
                    openDialogBody +=
$@"
          {FirstCharToLowerCase(property.Name)}: row?.{FirstCharToLowerCase(property.Name)} ?? 0,
";
                    break;
                case "Int64":
                    openDialogBody +=
$@"
          {FirstCharToLowerCase(property.Name)}: row?.{FirstCharToLowerCase(property.Name)} ?? null,
";
                    break;
                case "DateTime":
                    openDialogBody +=
$@"
          {FirstCharToLowerCase(property.Name)}: row?.{FirstCharToLowerCase(property.Name)} ?? null,
";
                    break;
                default:
                    break;
            }
        }


        openDialogFooter +=
$@"
          concurrencyStamp: row?.concurrencyStamp ?? """"
        }}
      }},
      width: ""46%"",
      draggable: true,
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(editForm, {{ ref: formRef }}),
      beforeSure: (done, {{ options }}) => {{
        const FormRef = formRef.value.getRef();
        const curData = options.props.formInline as FormItemProps;
        async function chores() {{
          message(`${{title}}成功`, {{ type: ""success"" }});
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }}
        FormRef.validate(async valid => {{
          if (valid) {{
            // 表单规则校验通过
            if (title === ""新增"") {{
              await add{typeName}(curData);
            }} else {{
              await update{typeName}(curData);
            }}
            chores();
          }}
        }});
      }}
    }});
  }}
";
        delete +=
$@"
  //删除
  async function handleDelete(row) {{
    await onbatchDelete{typeName}({{ {FirstCharToLowerCase(typeName)}Ids: [row.{FirstCharToLowerCase(typeName)}Id] }});
    message(`删除成功`, {{ type: ""success"" }});
    onSearch();
  }}
";
        onbatchDel +=
$@"
  /** 批量删除 */
  async function onbatchDel() {{
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    await onbatchDelete{typeName}({{ {FirstCharToLowerCase(typeName)}Ids: getKeyList(curSelected, ""id"") }});
    message(`删除成功`, {{ type: ""success"" }});
    onSearch();
    tableRef.value.getTableRef().clearSelection();
  }}
";
        selectionChangeNum +=
$@"
  /** 选择行 */
  function handleSelectionChange(val) {{
    selectedNum.value = val.length;
    // 重置表格高度
    tableRef.value.setAdaptive();
  }}
";
        selectionCancel +=
$@"
  /** 取消选择 */
  function onSelectionCancel() {{
    selectedNum.value = 0;
    // 用于多选表格，清空用户的选择
    tableRef.value.getTableRef().clearSelection();
  }}
";
        buttonClass +=
$@"
  //按钮样式类
  const buttonClass = computed(() => {{
    return [
      ""!h-[20px]"",
      ""reset-margin"",
      ""!text-gray-500"",
      ""dark:!text-white"",
      ""dark:hover:!text-primary""
    ];
  }});
";
        returnFunction +=
$@"
  //返回数据
  return {{
    form,
    loading,
    columns,
    dataList,
    pagination,
    selectedNum,
    buttonClass,
    onSearch,
    resetForm,
    onbatchDel,
    openDialog,
    handleDelete,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange
  }};
}}
";
        var header = import;
        var body = $@"" +
            $"" +
            $"{hookFunction}" +
            $"{constDefineHeader}" +
            $"{constDefineBody}" +
            $"{constDefineFooter}" +
            $"{onSearch}" +
            $"{resetFormSearch}" +
            $"{sizeChange}" +
            $"{currentChange}" +
            $"{columnsHeader}" +
            $"{columnsBody}" +
            $"{columnsFooter}" +
            $"{onMounted}" +
            $"{openDialogHeader}" +
            $"{openDialogBody}" +
            $"{openDialogFooter}" +
            $"{delete}" +
            $"{onbatchDel}" +
            $"{selectionChangeNum}" +
            $"{selectionCancel}" +
            $"{defineFunction}" +
            $"{buttonClass}";
        var footer = returnFunction;
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
    public static string GenerateRuleCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string fullPath = savePath;

        savePath = Path.Combine(fullPath, typeName.ToLower(), "utils");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, "rule.ts");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"import {{ reactive }} from ""vue"";
import type {{ FormRules }} from ""element-plus"";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{{";
        var body = "";
        PropertyInfo[] properties = type.GetProperties();

        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            bool isBreak = false;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    propertyTypeName = "string";
                    break;
                case "Boolean":
                    isBreak = true;
                    break;
                case "Int32":
                    isBreak = true;
                    break;
                case "Int64":
                    isBreak = true;
                    break;
                case "DateTime":
                    isBreak = true;
                    break;
                case "ICollection`1":
                    isBreak = true;
                    break;
                case "Nullable`1":
                    isBreak = true;
                    break;
                case "IReadOnlyCollection`1":
                    isBreak = true;
                    break;
                default:
                    isBreak = true;
                    break;
            }
            if (isBreak) continue;
            body +=
 $@"
  {FirstCharToLowerCase(property.Name)}: [{{ required: true, message: ""{description}为必填项"", trigger: ""blur"" }}],
";
        }

        body +=
$@"";

        var footer =
$@"}});";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
    public static string GenerateTypesCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string fullPath = savePath;
        // 构建目录路径
        savePath = Path.Combine(fullPath, typeName.ToLower(), "utils");
        Directory.CreateDirectory(savePath);
        // 构建文件路径
        string filePath = Path.Combine(savePath, "types.ts");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        var header =
$@"interface FormItemProps {{
  {FirstCharToLowerCase(typeName)}Id?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;";
        var body = "";
        PropertyInfo[] properties = type.GetProperties();

        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            bool isBreak = false;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    propertyTypeName = "string";
                    break;
                case "Boolean":
                    propertyTypeName = "boolean";
                    break;
                case "Int32":
                    propertyTypeName = "number";
                    break;
                case "Int64":
                    propertyTypeName = "number";
                    break;
                case "DateTime":
                    propertyTypeName = "string";
                    break;
                case "ICollection`1":
                    isBreak = true;
                    break;
                case "Nullable`1":
                    isBreak = true;
                    break;
                case "IReadOnlyCollection`1":
                    isBreak = true;
                    break;
                default:
                    isBreak = true;
                    break;
            }
            if (isBreak) continue;
            body +=
 $@"
  {FirstCharToLowerCase(property.Name)}: {propertyTypeName};
";
        }

        body +=
$@"  concurrencyStamp: string;
}}";

        var footer =
$@"interface FormProps {{formInline: FormItemProps;
}}

export type {{FormItemProps,FormProps}};";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
    public static string GenerateFormCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string fullPath = savePath;
        // 构建目录路径
        savePath = Path.Combine(fullPath, typeName.ToLower(), "form");
        Directory.CreateDirectory(savePath);
        // 构建文件路径
        string filePath = Path.Combine(savePath, "index.vue");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        PropertyInfo[] properties = type.GetProperties();

        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        var formPropsHeader = "";
        var formPropsBody = "";
        var formPropsFooter = "";
        var formHeader = "";
        var formBody = "";
        var formFooter = "";


        var header =
$@"<script setup lang=""ts"">
import {{ ref }} from ""vue"";
import ReCol from ""@/components/ReCol"";
import {{ formRules }} from ""../utils/rule"";
import {{ FormProps }} from ""../utils/types"";
import {{ usePublicHooks }} from ""@/views/system/hooks"";";

        formPropsHeader =
$@"
const props = withDefaults(defineProps<FormProps>(), {{
  formInline: () => ({{
    /** 用于判断是`新增`还是`修改` */
    title: ""新增"",
    {FirstCharToLowerCase(typeName)}Id: """",
";


        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            if (propertyTypeName == "String")
            {
                formPropsBody +=
$@"
    {FirstCharToLowerCase(property.Name)}: """",
";
            }
            if (propertyTypeName == "Boolean")
            {
                formPropsBody +=
$@"     
    {FirstCharToLowerCase(property.Name)}: null,
";
            }
            if (propertyTypeName == "DateTime")
            {
                formPropsBody +=
$@"     
    {FirstCharToLowerCase(property.Name)}: """",
";
            }
            if (propertyTypeName == "Int32" || propertyTypeName == "Int64")
            {
                formPropsBody +=
$@"     
    {FirstCharToLowerCase(property.Name)}: 0,
";
            }

        }


        formPropsFooter =
$@"
    concurrencyStamp: """"
  }})
}});

const ruleFormRef = ref();
const {{ switchStyle }} = usePublicHooks();
const newFormInline = ref(props.formInline);

function getRef() {{
  return ruleFormRef.value;
}}

defineExpose({{ getRef }});
</script>
";

        formHeader +=
$@"
<template>
  <el-form
    ref=""ruleFormRef""
    :model=""newFormInline""
    :rules=""formRules""
    label-width=""82px""
  >
    <el-row :gutter=""30"">
";

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            if (propertyTypeName == "String")
            {
                formBody +=
$@"
      <re-col :value=""12"" :xs=""24"" :sm=""24"">
        <el-form-item label=""{description}"" prop=""{FirstCharToLowerCase(property.Name)}"">
          <el-input
            v-model=""newFormInline.{FirstCharToLowerCase(property.Name)}""
            clearable
            placeholder=""请输入{description}""
          />
        </el-form-item>
      </re-col>
";
            }
            if (propertyTypeName == "Boolean")
            {
                formBody +=
$@"
      <re-col :value=""12"" :xs=""24"" :sm=""24"">
        <el-form-item label=""{description}"">
          <el-switch
            v-model=""newFormInline.{FirstCharToLowerCase(property.Name)}""
            inline-prompt
            :active-value=""true""
            :inactive-value=""false""
            active-text=""是""
            inactive-text=""否""
            :style=""switchStyle""
          />
        </el-form-item>
      </re-col>
";
            }
            if (propertyTypeName == "DateTime")
            {
                formBody +=
$@"
      <re-col :value=""12"" :xs=""24"" :sm=""24"">
        <el-form-item label=""{description}"" prop=""{FirstCharToLowerCase(property.Name)}"">
          <el-date-picker
            v-model=""newFormInline.{FirstCharToLowerCase(property.Name)}""
            type=""datetime""
            placeholder=""选择一个{description}""
          />
        </el-form-item>
      </re-col>
";
            }
            if (propertyTypeName == "Int32" || propertyTypeName == "Int64")
            {
                formBody +=
$@"
      <re-col :value=""12"" :xs=""24"" :sm=""24"">
        <el-form-item label=""类型"" prop=""type"">
          <el-input
            v-model=""newFormInline.{FirstCharToLowerCase(property.Name)}""
            clearable
            placeholder=""请输入类型""
            type=""number""
          />
        </el-form-item>
      </re-col>
";
            }

        }

        formFooter +=
$@"
    </el-row>
  </el-form>
</template>
";

        var body = $@"
                        {formPropsHeader}
                        {formPropsBody}
                        {formPropsFooter}
                        {formHeader}
                        {formBody}
                        {formFooter}";
        var footer =
$@"";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
    public static string GenerateIndexCode(Type type, string nameSpace, string savePath)
    {
        var typeName = type.Name;
        string fullPath = savePath;
        // 构建目录路径
        savePath = Path.Combine(fullPath, typeName.ToLower());
        Directory.CreateDirectory(savePath);
        // 构建文件路径
        string filePath = Path.Combine(savePath, "index.vue");
        var desc = GenrateCodeHelper.GetTypeDescription(type);

        PropertyInfo[] properties = type.GetProperties();

        string[] ignoreFields = new string[]
        {
            "Id",
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
            "ConcurrencyStamp"
        };

        var formSearchHeader = "";
        var formSearchBody = "";
        var formSearchFooter = "";

        var header =
$@"
<script setup lang=""ts"">
import {{ ref }} from ""vue"";
import {{ use{typeName} }} from ""./utils/hook"";
import {{ useRenderIcon }} from ""@/components/ReIcon/src/hooks"";
import {{ PureTableBar }} from ""@/components/RePureTableBar"";
import {{ hasAuth }} from ""@/router/utils"";

import Search from ""@iconify-icons/ep/search"";
import Delete from ""@iconify-icons/ep/delete"";
import EditPen from ""@iconify-icons/ep/edit-pen"";
import Refresh from ""@iconify-icons/ep/refresh"";
import AddFill from ""@iconify-icons/ri/add-circle-line"";

defineOptions({{
  name: ""{typeName}""
}});
const treeRef = ref();
const formRef = ref();
const tableRef = ref();
const {{
  form,
  loading,
  columns,
  dataList,
  pagination,
  selectedNum,
  onSearch,
  resetForm,
  onbatchDel,
  openDialog,
  handleDelete,
  handleSizeChange,
  onSelectionCancel,
  handleCurrentChange,
  handleSelectionChange
}} = use{typeName}(tableRef, treeRef);
</script>
<template>
  <div
    class=""flex justify-between""
    v-if=""hasAuth('api:{typeName.ToLower()}:paginationquery')""
  >
";

        formSearchHeader =
$@"
    <div class=""w-[calc(100%-0px)]"">
      <el-form
        ref=""formRef""
        :inline=""true""
        :model=""form""
        class=""search-form bg-bg_color w-[99/100] pl-8 pt-[12px]""
      >
";
        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var propertyTypeName = property.PropertyType.Name;
            var description = property.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;
            if (property.Name.Contains("Id")) continue;
            if (ignoreFields.Contains(property.Name)) continue;

            if (propertyTypeName == "String")
            {
                formSearchBody +=
$@"
        <el-form-item label=""{description}："" prop=""{FirstCharToLowerCase(property.Name)}"">
          <el-input
            v-model=""form.{FirstCharToLowerCase(property.Name)}""
            placeholder=""请输入{description}""
            clearable
            class=""!w-[160px]""
          />
        </el-form-item>
";
            }
            if (propertyTypeName == "Boolean")
            {
                formSearchBody +=
$@"
<el-form-item label=""{description}："" prop=""{FirstCharToLowerCase(property.Name)}"">
          <el-select
            v-model=""form.{FirstCharToLowerCase(property.Name)}""
            placeholder=""请选{description}""
            clearable
            class=""!w-[160px]""
          >
            <el-option label=""是"" :value=""true"" />
            <el-option label=""否"" :value=""false"" />
          </el-select>
        </el-form-item>
";
            }
        }
        formSearchFooter +=
$@"
        <el-form-item>
          <el-button :icon=""useRenderIcon(Search)"" type=""primary"" @click=""onSearch""> 搜索 </el-button>
          <el-button :icon=""useRenderIcon(Refresh)"" @click=""resetForm(formRef)"">
            重置
          </el-button>
        </el-form-item>
      </el-form>
";

        var body = $@"
                        {formSearchHeader}
                        {formSearchBody}
                        {formSearchFooter}";
        var footer =
$@"
      <PureTableBar title=""{desc}管理"" :columns=""columns"" @refresh=""onSearch"">
        <template #buttons>
          <el-button
            type=""primary""
            :icon=""useRenderIcon(AddFill)""
            @click=""openDialog()""
            v-if=""hasAuth('api:{typeName.ToLower()}:add')""
          >
            新增
          </el-button>
        </template>
        <template v-slot=""{{ size, dynamicColumns }}"">
          <div
            v-if=""selectedNum > 0""
            v-motion-fade
            class=""bg-[var(--el-fill-color-light)] w-full h-[46px] mb-2 pl-4 flex items-center""
          >
            <div class=""flex-auto"">
              <span
                style=""font-size: var(--el-font-size-base)""
                class=""text-[rgba(42,46,54,0.5)] dark:text-[rgba(220,220,242,0.5)]""
              >
                已选 {{{{ selectedNum }}}} 项
              </span>
              <el-button type=""primary"" text @click=""onSelectionCancel"">
                取消选择
              </el-button>
            </div>
            <el-popconfirm title=""是否确认删除?"" @confirm=""onbatchDel"">
              <template #reference>
                <el-button
                  type=""danger""
                  text
                  class=""mr-1""
                  v-if=""hasAuth('api:{typeName.ToLower()}:delete')""
                >
                  批量删除
                </el-button>
              </template>
            </el-popconfirm>
          </div>
          <pure-table
            row-key=""id""
            ref=""tableRef""
            adaptive
            align-whole=""center""
            table-layout=""auto""
            :loading=""loading""
            :size=""size""
            :data=""dataList""
            :columns=""dynamicColumns""
            :pagination=""pagination""
            :paginationSmall=""size === 'small' ? true : false""
            :header-cell-style=""{{
              background: 'var(--el-fill-color-light)',
              color: 'var(--el-text-color-primary)'
            }}""
            @selection-change=""handleSelectionChange""
            @page-size-change=""handleSizeChange""
            @page-current-change=""handleCurrentChange""
          >
            <template #operation=""{{ row }}"">
              <el-button
                class=""reset-margin""
                link
                type=""primary""
                :size=""size""
                :icon=""useRenderIcon(EditPen)""
                @click=""openDialog('编辑', row)""
                v-if=""hasAuth('api:{typeName.ToLower()}:update')""
              >
                修改
              </el-button>
              <el-popconfirm
                :title=""`是否确认删除编号为${{row.{FirstCharToLowerCase(typeName)}Id}}的这条数据`""
                @confirm=""handleDelete(row)""
              >
                <template #reference>
                  <el-button
                    class=""reset-margin""
                    link
                    type=""primary""
                    :size=""size""
                    :icon=""useRenderIcon(Delete)""
                    v-if=""hasAuth('api:{typeName.ToLower()}:delete')""
                  >
                    删除
                  </el-button>
                </template>
              </el-popconfirm>
            </template>
          </pure-table>
        </template>
      </PureTableBar>
    </div>
  </div>
</template>

<style scoped lang=""scss"">
:deep(.el-dropdown-menu__item i) {{
  margin: 0;
}}

:deep(.el-button:focus-visible) {{
  outline: none;
}}

.search-form {{
  :deep(.el-form-item) {{
    margin-bottom: 12px;
  }}
}}
</style>
";
        var code = $"{header}{body}{footer}";
        GenrateCodeHelper.SaveTextToFile(code, filePath);
        return filePath;
    }
}
