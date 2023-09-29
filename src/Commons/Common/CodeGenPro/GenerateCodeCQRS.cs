using System.IO;
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
    public static bool ShouldGenerateToString(string typeName, IEnumerable<string>? excludeTypeName = null)
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
             "ConcurrencyStamp"
        };

        if (excludeTypeName != null && excludeTypeName.Any())
        {
            ignoreTypeNames.RemoveAll(x => excludeTypeName.Contains(x));
        }

        return !ignoreTypeNames.Contains(typeName);
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

        // 将 Nullable<T> 转换为 T?
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            typeName = typeName.Replace("Nullable<", "").Replace(">", "?");
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
}

/// <summary>
/// 生成CQRS代码
/// </summary>
public class GenerateCodeCQRS
{
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

    public static string GenerateCachingCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Caching";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}CacheKey.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using Microsoft.Extensions.Primitives;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
namespace {nameSpace}.{type.Name}s.Caching;

public static class {type.Name}CacheKey
{{
    public const string GetAllCacheKey = ""all-{type.Name}s"";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetByIdCacheKey(long id)
    {{
        return $""Get{type.Name}ById,{{id}}"";
    }}

    static {type.Name}CacheKey()
    {{
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }}

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {{
        return $""{type.Name}sWithPaginationQuery,{{parameters}}"";
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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateAddCommandCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Commands\\Add";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\Add{type.Name}Command.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using System.ComponentModel.DataAnnotations;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using {nameSpace}.{type.Name}s.Caching;
using Domain.Entities;
using Masuit.Tools.Systems;
using Microsoft.Extensions.Options;

namespace {nameSpace}.{type.Name}s.Commands.Add;

/// <summary>
/// 添加{desc}
/// </summary>
[Map(typeof({type.Name}))]
public class Add{type.Name}Command : IRequest<Result<long>>
{{";
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
                    propertyTypeName = "bool";
                    break;
                case "Int32":
                    propertyTypeName = "int";
                    break;
                case "Int64":
                    propertyTypeName = "long";
                    break;
                case "DateTime":
                    propertyTypeName = "DateTime";
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
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {property.Name} {{ get; set; }}";

        }
        body +=
$@"
}}";

        var footer =
$@"
/// <summary>
/// 处理程序
/// </summary>
public class Add{type.Name}CommandHandler : IRequestHandler<Add{type.Name}Command, Result<long>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Add{type.Name}CommandHandler(
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
    public async Task<Result<long>> Handle(Add{type.Name}Command request, CancellationToken cancellationToken)
    {{
        var {type.Name.ToLower()} = _mapper.Map<{type.Name}>(request);
        {type.Name.ToLower()}.AddDomainEvent(new CreatedEvent<{type.Name}>({type.Name.ToLower()}));
        await _context.{type.Name}s.AddAsync({type.Name.ToLower()});
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync({type.Name.ToLower()}.Id, isSuccess, new string[] {{ ""操作失败"" }});
    }}
}}";
        var code = $"{header}{body}{footer}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateUpdateCommandCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Commands\\Update";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\Update{type.Name}Command.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using {nameSpace}.{type.Name}s.Caching;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{type.Name}s.Commands.Update;


/// <summary>
/// 修改{desc}
/// </summary>
[Map(typeof({type.Name}))]
public class Update{type.Name}Command : IRequest<Result<long>>
{{
";
        var body = "";
        PropertyInfo[] properties = type.GetProperties();
        string[] ignoreFields = new string[]
       {
            "DeletedBy",
            "Deleted",
            "CreatedBy",
            "LastModified",
            "LastModifiedBy",
            "LastModifiedBy",
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
            if (ignoreFields.Contains(property.Name)) continue;

            switch (propertyTypeName)
            {
                case "String":
                    propertyTypeName = "string";
                    break;
                case "Boolean":
                    propertyTypeName = "bool";
                    break;
                case "Int32":
                    propertyTypeName = "int";
                    break;
                case "Int64":
                    propertyTypeName = "long";
                    break;
                case "DateTime":
                    propertyTypeName = "DateTime";
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
            if (property.Name == "Id")
            {
                body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {type.Name}{property.Name} {{ get; set; }}";

            }
            else
            {
                body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName} {property.Name} {{ get; set; }}";

            }
        }
        body +=
$@"
}}";

        var footer = $@"

/// <summary>
/// 处理程序
/// </summary>
public class Update{type.Name}CommandHandler : IRequestHandler<Update{type.Name}Command, Result<long>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Update{type.Name}CommandHandler(
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
    public async Task<Result<long>> Handle(Update{type.Name}Command request, CancellationToken cancellationToken)
    {{
        var {type.Name.ToLower()} = await _context.{type.Name}s
           .SingleOrDefaultAsync(x => x.Id == request.{type.Name}Id && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
           ?? throw new NotFoundException($""数据【{{request.{type.Name}Id}}-{{request.ConcurrencyStamp}}】未找到"");

        {type.Name.ToLower()} = _mapper.Map(request, {type.Name.ToLower()});
        {type.Name.ToLower()}.AddDomainEvent(new UpdatedEvent<{type.Name}>({type.Name.ToLower()}));
        _context.{type.Name}s.Update({type.Name.ToLower()});
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync({type.Name.ToLower()}.Id, isSuccess, new string[] {{ ""操作失败"" }});
    }}
}}
";


        var code = $"{header}{body}{footer}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateDeleteCommandCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Commands\\Delete";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\Delete{type.Name}Command.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using {nameSpace}.{type.Name}s.Caching;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using Domain.Entities;

namespace {nameSpace}.{type.Name}s.Commands.Delete;

/// <summary>
/// 删除{desc}
/// </summary>
public class Delete{type.Name}Command : IRequest<Result<bool>>
{{
";
        var body =
$@"  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description(""唯一标识"")]
        public List<long> {type.Name}Ids {{ get; set; }}";
        body +=
$@"
}}";

        var footer = $@"

/// <summary>
/// 处理程序
/// </summary>
public class Delete{type.Name}CommandHandler : IRequestHandler<Delete{type.Name}Command, Result<bool>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Delete{type.Name}CommandHandler(
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
    public async Task<Result<bool>> Handle(Delete{type.Name}Command request, CancellationToken cancellationToken)
    {{
        var {type.Name.ToLower()}sToDelete = await _context.{type.Name}s
            .Where(x => request.{type.Name}Ids.Contains(x.Id))
            .ToListAsync();

        if ({type.Name.ToLower()}sToDelete.Any())
        {{
            _context.{type.Name}s.RemoveRange({type.Name.ToLower()}sToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {{""操作失败""}});
        }}

        return await Result<bool>.FailureAsync(new string[] {{ ""没有找到需要删除的数据"" }});
    }}
}}";
        var code = $"{header}{body}{footer}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateDTOsCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\DTOs";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}Dto.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using Domain.Entities;
using Domain.Enums;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{type.Name}s.DTOs
{{
    [Map(typeof({type.Name}))]
    public class {type.Name}Dto
    {{
";

        var body =
$@"     
        /// <summary>
        /// 唯一标识
        /// </summary>
        public long? Id {{ get; set; }}

        /// <summary>
        /// 唯一标识
        /// </summary>
        public long? {type.Name}Id 
        {{
            get 
            {{
                return Id;
            }}
        }}";

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
            if (propertyTypeName == type.Name) continue;

            if (propertyTypeName == "String")
            {
                propertyTypeName = "string";
            }
            if (propertyTypeName == "Boolean")
            {
                propertyTypeName = "bool";
            }
            if (propertyTypeName == "Int32")
            {
                propertyTypeName = "int";
            }
            if (propertyTypeName == "ICollection`1")
            {
                continue;
            }
            if (propertyTypeName == "IReadOnlyCollection`1")
            {
                continue;
            }
            if (propertyTypeName == "Nullable`1")
            {
                string a = property.ToString();
                if (property.ToString().Contains("DateTimeOffset"))
                {
                    propertyTypeName = "DateTimeOffset";
                }
                if (property.ToString().Contains("DateTime"))
                {
                    propertyTypeName = "DateTime";
                }
                if (property.ToString().Contains("Int64"))
                {
                    propertyTypeName = "long";
                }
            }

            body +=
 $@"
        
        /// <summary>
        /// {description}
        /// </summary>
        [Description(""{description}"")]
        public {propertyTypeName}? {property.Name} {{ get; set; }}";

        }



        var footer =
$@"

        /// <summary>
        /// 并发标记
        /// </summary>
        [Description(""并发标记"")]
        public string? ConcurrencyStamp {{ get; set; }}
    }}
}}";
        var code = $"{header}{body}{footer}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;

    }

    public static string GenerateEventHandlersCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\EventHandlers";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}CreatedEventHandler.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
namespace {nameSpace}.{type.Name}s.EventHandlers;

public class {type.Name}CreatedEventHandler : INotificationHandler<CreatedEvent<{type.Name}>>
{{
    private readonly ILogger<{type.Name}CreatedEventHandler> _logger;

    public {type.Name}CreatedEventHandler(
        ILogger<{type.Name}CreatedEventHandler> logger
    )
    {{
        _logger = logger;
    }}

    public Task Handle(CreatedEvent<{type.Name}> notification, CancellationToken cancellationToken)
    {{
        return Task.CompletedTask;
    }}
}}
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateQueriesGetAllCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Queries\\GetAll";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\GetAll{type.Name}Query.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using {nameSpace}.{type.Name}s.Caching;
using {nameSpace}.{type.Name}s.DTOs;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using AutoMapper.QueryableExtensions;

namespace {nameSpace}.{type.Name}s.Queries.GetAll;

public class GetAll{type.Name}sQuery : IRequest<Result<IEnumerable<{type.Name}Dto>>>
{{
}}

public class GetAll{type.Name}sQueryHandler :
    IRequestHandler<GetAll{type.Name}sQuery, Result<IEnumerable<{type.Name}Dto>>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAll{type.Name}sQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {{
        _context = context;
        _mapper = mapper;
    }}

    public async Task<Result<IEnumerable<{type.Name}Dto>>> Handle(GetAll{type.Name}sQuery request, CancellationToken cancellationToken)
    {{
        var data = await _context.{type.Name}s
            .ProjectTo<{type.Name}Dto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<{type.Name}Dto>>.SuccessAsync(data);
    }}
}}

";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateQueriesGetByIdCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Queries\\GetById";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\Get{type.Name}QueryById.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using Application.Common.Extensions;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using {nameSpace}.{type.Name}s.Caching;
using {nameSpace}.{type.Name}s.DTOs;
using {nameSpace}.{type.Name}s.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace {nameSpace}.{type.Name}s.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
public class Get{type.Name}QueryById : IRequest<Result<{type.Name}Dto>>
{{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = ""唯一标识必填的"")]
    public long {type.Name}Id {{ get; set; }}
}}

/// <summary>
/// 处理程序
/// </summary>
public class Get{type.Name}ByIdQueryHandler :IRequestHandler<Get{type.Name}QueryById, Result<{type.Name}Dto>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Get{type.Name}ByIdQueryHandler(
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
    public async Task<Result<{type.Name}Dto>> Handle(Get{type.Name}QueryById request, CancellationToken cancellationToken)
    {{
        var {type.Name.ToLower()} = await _context.{type.Name}s.ApplySpecification(new {type.Name}ByIdSpec(request.{type.Name}Id))
                     .ProjectTo<{type.Name}Dto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($""唯一标识: [{{request.{type.Name}Id}}] 未找到"");
        return await Result<{type.Name}Dto>.SuccessAsync({type.Name.ToLower()});
    }}
}}
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateQueriesPaginationCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Queries\\Pagination";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}sWithPaginationQuery.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using Application.Common.Extensions;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using {nameSpace}.{type.Name}s.Caching;
using {nameSpace}.{type.Name}s.DTOs;
using {nameSpace}.{type.Name}s.Specifications;

namespace {nameSpace}.{type.Name}s.Queries.Pagination;

/// <summary>
/// {desc}分页查询
/// </summary>
public class {type.Name}sWithPaginationQuery : {type.Name}AdvancedFilter, IRequest<Result<PaginatedData<{type.Name}Dto>>>
{{
    [JsonIgnore]
    public {type.Name}AdvancedPaginationSpec Specification => new {type.Name}AdvancedPaginationSpec(this);
}}

/// <summary>
/// 处理程序
/// </summary>
public class {type.Name}sWithPaginationQueryHandler :
    IRequestHandler<{type.Name}sWithPaginationQuery, Result<PaginatedData<{type.Name}Dto>>>
{{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public {type.Name}sWithPaginationQueryHandler(
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
    public async Task<Result<PaginatedData<{type.Name}Dto>>> Handle(
        {type.Name}sWithPaginationQuery request,
        CancellationToken cancellationToken)
    {{
        var {type.Name.ToLower()}s = await _context.{type.Name}s
            .OrderBy($""{{request.OrderBy}} {{request.SortDirection}}"")
            .ProjectToPaginatedDataAsync<{type.Name}, {type.Name}Dto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<{type.Name}Dto>>.SuccessAsync({type.Name.ToLower()}s);
    }}
}}
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateSpecificationsFilterCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Specifications";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}AdvancedFilter.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
namespace {nameSpace}.{type.Name}s.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class {type.Name}AdvancedFilter : PaginationFilter
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
            if (propertyTypeName == type.Name) continue;

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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateSpecificationsPaginationSpecCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Specifications";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}AdvancedPaginationSpec.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"using Ardalis.Specification;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using Masuit.Tools;

namespace {nameSpace}.{type.Name}s.Specifications;

public class {type.Name}AdvancedPaginationSpec : Specification<{type.Name}>
{{
    public {type.Name}AdvancedPaginationSpec({type.Name}AdvancedFilter filter)
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
            if (propertyTypeName == type.Name) continue;

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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateSpecificationsByIdSpecCode(Type type, string nameSpace, string savePath)
    {
        savePath = $"{savePath}\\{type.Name}s\\Specifications";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\{type.Name}ByIdSpec.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using Ardalis.Specification;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};

namespace {nameSpace}.{type.Name}s.Specifications;

public class {type.Name}ByIdSpec : Specification<{type.Name}>
{{
    public {type.Name}ByIdSpec(long id)
    {{
        Query.Where(q => q.Id == id);
    }}
}}
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }

    public static string GenerateControllerCode(Type type, string nameSpace, string savePath)
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

        savePath = $"{fullPath}{targetSubstring}\\WebAPI\\Controllers";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\{type.Name}Controller.cs";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var body =
$@"using {nameSpace}.{type.Name}s.DTOs;
using {nameSpace}.{type.Name}s.Commands.Add;
using {nameSpace}.{type.Name}s.Commands.Delete;
using {nameSpace}.{type.Name}s.Commands.Update;
using {nameSpace}.{type.Name}s.Queries.Pagination;
using {RemoveSuffix($"{type.FullName}", $".{type.Name}")};
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// {desc}
/// </summary>
public class {type.Name}Controller : ApiControllerBase
{{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost(""PaginationQuery"")]

    public async Task<Result<PaginatedData<{type.Name}Dto>>> PaginationQuery({type.Name}sWithPaginationQuery query)
    {{
        return await Mediator.Send(query);
    }}

    /// <summary>
    /// 创建{desc}
    /// </summary>
    /// <returns></returns>
    [HttpPost(""Add"")]

    public async Task<Result<long>> Add(Add{type.Name}Command command)
    {{
        return await Mediator.Send(command);
    }}

    /// <summary>
    /// 修改{desc}
    /// </summary>
    /// <returns></returns>
    [HttpPut(""Update"")]
    public async Task<Result<long>> Update(Update{type.Name}Command command)
    {{
        return await Mediator.Send(command);
    }}

    /// <summary>
    /// 删除{desc}
    /// </summary>
    /// <returns></returns>
    [HttpDelete(""Delete"")]
    public async Task<Result<bool>> Delete(Delete{type.Name}Command command)
    {{
        return await Mediator.Send(command);
    }}
}}
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
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
        var filePath = $@"{savePath}\\{type.Name.ToLower()}.ts";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

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
export const get{type.Name}List = (data?: object) => {{
  return http.request<ResultTable>(""post"", ""/api/{type.Name}/PaginationQuery"", {{
    data
  }});
}};

/** 新增 */
export const add{type.Name} = (data?: object) => {{
  return http.request<Result>(""post"", ""/api/{type.Name}/Add"", {{
    data
  }});
}};

/** 修改 */
export const update{type.Name} = (data?: object) => {{
  return http.request<Result>(""put"", ""/api/{type.Name}/Update"", {{
    data
  }});
}};

/** 批量删除 */
export const onbatchDelete{type.Name} = (data?: object) => {{
  return http.request<Result>(""delete"", ""/api/{type.Name}/Delete"", {{
    data
  }});
}};
";
        var code = $"{body}";
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
    public static string GenerateHookCode(Type type, string nameSpace, string savePath)
    {
        string[] folders = savePath.Split(Path.DirectorySeparatorChar);
        string lastFolder = folders[folders.Length - 1];

        string fullPath = savePath;
        savePath = $"{fullPath}\\{type.Name.ToLower()}\\utils";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\hook.tsx";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

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
  get{type.Name}List,
  add{type.Name},
  update{type.Name},
  onbatchDelete{type.Name}
}} from ""@/api/{lastFolder}/{type.Name.ToLower()}"";
import {{ type PaginationProps }} from ""@pureadmin/table"";
import {{ usePublicHooks }} from ""../../hooks"";
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
export function useTestTable(tableRef: Ref, treeRef: Ref) {{
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
    const {{ data }} = await get{type.Name}List(toRaw(form));
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
        const auths = [""api:{type.Name.ToLower()}:update"", ""api:{type.Name.ToLower()}:delete""];
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
          {FirstCharToLowerCase(type.Name)}Id: row?.{FirstCharToLowerCase(type.Name)}Id ?? """",
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
              await addTestTable(curData);
            }} else {{
              await updateTestTable(curData);
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
    await onbatchDelete{type.Name}({{ {FirstCharToLowerCase(type.Name)}Ids: [row.{FirstCharToLowerCase(type.Name)}Id] }});
    message(`删除成功`, {{ type: ""success"" }});
    onSearch();
  }}
";
        onbatchDel +=
$@"
  /** 批量删除 */
  async function onbatchDel() {{
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    await onbatchDelete{type.Name}({{ {FirstCharToLowerCase(type.Name)}Ids: getKeyList(curSelected, ""id"") }});
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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
    public static string GenerateRuleCode(Type type, string nameSpace, string savePath)
    {
        string fullPath = savePath;
        savePath = $"{fullPath}\\{type.Name.ToLower()}\\utils";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\rule.ts";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
    public static string GenerateTypesCode(Type type, string nameSpace, string savePath)
    {
        string fullPath = savePath;
        savePath = $"{fullPath}\\{type.Name.ToLower()}\\utils";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\types.ts";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

        var header =
$@"interface FormItemProps {{
  {FirstCharToLowerCase(type.Name)}Id?: string;
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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
    public static string GenerateFormCode(Type type, string nameSpace, string savePath)
    {
        string fullPath = savePath;
        savePath = $"{fullPath}\\{type.Name.ToLower()}\\form";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}\\index.vue";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

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
import {{ usePublicHooks }} from ""../../hooks"";";

        formPropsHeader =
$@"
const props = withDefaults(defineProps<FormProps>(), {{
  formInline: () => ({{
    /** 用于判断是`新增`还是`修改` */
    title: ""新增"",
    {FirstCharToLowerCase(type.Name)}Id: """",
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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
    public static string GenerateIndexCode(Type type, string nameSpace, string savePath)
    {
        string fullPath = savePath;
        savePath = $"{fullPath}\\{type.Name.ToLower()}\\";
        Directory.CreateDirectory(savePath);
        var filePath = $@"{savePath}index.vue";
        var desc = type.CustomAttributes?
            .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
            .ConstructorArguments?
            .FirstOrDefault().Value;

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
import {{ use{type.Name} }} from ""./utils/hook"";
import {{ useRenderIcon }} from ""@/components/ReIcon/src/hooks"";
import {{ PureTableBar }} from ""@/components/RePureTableBar"";
import {{ hasAuth }} from ""@/router/utils"";

import Search from ""@iconify-icons/ep/search"";
import Delete from ""@iconify-icons/ep/delete"";
import EditPen from ""@iconify-icons/ep/edit-pen"";
import Refresh from ""@iconify-icons/ep/refresh"";
import AddFill from ""@iconify-icons/ri/add-circle-line"";

defineOptions({{
  name: ""TestTable""
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
}} = use{type.Name}(tableRef, treeRef);
</script>
<template>
  <div
    class=""flex justify-between""
    v-if=""hasAuth('api:{type.Name.ToLower()}:paginationquery')""
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
            v-if=""hasAuth('api:{type.Name.ToLower()}:add')""
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
                  v-if=""hasAuth('api:{type.Name.ToLower()}:delete')""
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
                v-if=""hasAuth('api:{type.Name.ToLower()}:update')""
              >
                修改
              </el-button>
              <el-popconfirm
                :title=""`是否确认删除编号为${{row.{FirstCharToLowerCase(type.Name)}Id}}的这条数据`""
                @confirm=""handleDelete(row)""
              >
                <template #reference>
                  <el-button
                    class=""reset-margin""
                    link
                    type=""primary""
                    :size=""size""
                    :icon=""useRenderIcon(Delete)""
                    v-if=""hasAuth('api:{type.Name.ToLower()}:delete')""
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
        using (FileStream fs = System.IO.File.Create(filePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(code);
            fs.Write(info, 0, info.Length);
        }
        return filePath;
    }
}
