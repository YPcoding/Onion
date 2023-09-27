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
        public static string GenerateAddCommandCode(Type type, string nameSpace, string savePath)
        {
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

        public static string GenerateCachingCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateDTOsCode(Type type, string nameSpace, string savePath) 
        {
            return "";
        }

        public static string GenerateEventHandlersCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateQueriesGetAllCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateQueriesGetByIdCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateQueriesPaginationCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateSpecificationsFilterCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateSpecificationsPaginationSpecCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateSpecificationsByIdSpecCode(Type type, string nameSpace, string savePath)
        {
            return "";
        }

        public static string GenerateControllerCode(Type type, string nameSpace, string savePath)
        {
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
