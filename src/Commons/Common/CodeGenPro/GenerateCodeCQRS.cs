using System.Reflection;
using System.Text;

/// <summary>
/// 代码生成器
/// </summary>
namespace Common.CodeGenPro
{
    /// <summary>
    /// 生成CQRS代码
    /// </summary>
    public class GenerateCodeCQRS
    {
        public static Type GetTypeByFullClassName(string fullClassName)
        {
            string className = fullClassName;
            Assembly assembly = Assembly.Load(className.Split('.')[0].ToString());
            return assembly.GetType(className)!;
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
$@"/// <summary>
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

            // 遍历属性并输出它们的名称和类型
            foreach (PropertyInfo property in properties)
            {
                var propertyTypeName = property.PropertyType.Name;
                var description = property.CustomAttributes?
                .FirstOrDefault(x => x.AttributeType.Name == "DescriptionAttribute")?
                .ConstructorArguments?
                .FirstOrDefault().Value;
                bool isBreak = false;

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
            return await Result<bool>.FailureAsync(new string[] {{ ""操作失败"" }});
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
$@"{nameSpace}.Features.{type.Name}s.EventHandlers;

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
$@"namespace {nameSpace}.{type.Name}s.Specifications;

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
            savePath = $"{savePath}\\{type.Name}s\\Specifications";
            Directory.CreateDirectory(savePath);
            var filePath = $@"{savePath}\{type.Name}Controller.cs";
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
        public static Type GetTypeByFullClassName(string fullClassName = "Domain.Entities.Identity.User")
        {
            string className = fullClassName;
            Assembly assembly = Assembly.Load(className.Split('.')[0].ToString());
            return assembly.GetType(className)!;
        }

        public static string GenerateHookCode(Type type, string nameSpace, string savePath) 
        {
            return "";
        }

        public static string GenerateRuleCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateTypesCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateFormCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateIndexCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }
    }
}
