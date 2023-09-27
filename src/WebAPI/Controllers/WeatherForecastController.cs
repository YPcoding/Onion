using Application.Common.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Masuit.Tools;
using Masuit.Tools.Reflection;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System;
using System.Text;
using static Application.Common.Helper.WebApiDocHelper;

namespace WebAPI.Controllers;

/// <summary>
/// 测试接口
/// </summary>
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IApplicationDbContext _context;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 接口调试
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [AllowAnonymous]
    public async Task<int> Get()
    {
        //List<ControllerInfo> controllers = GetWebApiControllersWithActions();
        //var permissions = await _context.Permissions.ToListAsync();
        //var menuGroups = controllers.GroupBy(x => x.Group).ToList();

        //var permissionsToAdd = new List<Permission>();

        //foreach (var permissionMenus in menuGroups)
        //{
        //    var permissionMenu = permissions
        //        .Where(x => x.Group == permissionMenus.Key && x.SuperiorId == null)
        //        .FirstOrDefault();
        //    if (permissionMenu == null)
        //    {
        //        permissionMenu = new Permission();
        //        if (permissionMenus.Key == "系统管理")
        //        {
        //            permissionMenu.Path = "/System";
        //            permissionMenu.Id = SnowFlake.GetInstance().GetLongId();
        //            var menu = new Permission("系统管理", "系统管理", $"{permissionMenu.Path.ToLower()}", 0, "", PermissionType.Menu, "", "lollipop")
        //            {
        //                Id = permissionMenu.Id
        //            };
        //            permissionsToAdd.Add(menu);
        //        }
        //        else
        //        {
        //            int count = permissions.GroupBy(x => x.Group).Count();
        //            string path = permissionMenus?.FirstOrDefault()!.ControllerName.ToLower()!;
        //            var menuId = SnowFlake.GetInstance().GetLongId();
        //            var menu = new Permission(permissionMenus!.Key, permissionMenus.Key, $"/{path}", count++, "", PermissionType.Menu, "", "lollipop")
        //            {
        //                Id = menuId
        //            };
        //            permissionsToAdd.Add(menu);
        //        }
        //    }
        //    foreach (var permissionPage in permissionMenus)
        //    {
        //        var pageId = SnowFlake.GetInstance().GetLongId();
        //        var pagePath = $"{permissionMenu.Path?.ToLower()}/{permissionPage.ControllerName}/index";
        //        var pageName = $"{permissionMenu.Path?.Replace("/", "")}{permissionPage.ControllerName}Page";
        //        var page = new Permission(permissionPage.ControllerDescription, permissionPage.ControllerDescription, pagePath, 0, "", PermissionType.Page, pageName, "")
        //        {
        //            Id = pageId,
        //            SuperiorId = permissionMenu.Id
        //        };
        //        var existPage = permissions
        //        .Where(x => x.Code == page.Code && x.Type == PermissionType.Page)
        //        .FirstOrDefault();
        //        if (existPage == null)
        //        {
        //            existPage = new Permission();
        //            existPage = page;
        //            permissionsToAdd.Add(page);
        //        }

        //        foreach (var permissionDot in permissionPage.Actions)
        //        {
        //            var path = $"api/{permissionPage.ControllerName}/{permissionDot.Route}";
        //            if (permissionDot.Description.IsNullOrEmpty()) continue;
        //            var dot = new Permission(permissionPage.ControllerDescription, permissionDot.Description, path, 0, permissionDot.HttpMethods, PermissionType.Dot, "", "")
        //            {
        //                Id = SnowFlake.GetInstance().GetLongId(),
        //                SuperiorId = existPage.Id
        //            };
        //            var existDot = permissions
        //                .Where(x => x.Code == page.Code && x.Type == PermissionType.Dot)
        //                .FirstOrDefault();
        //            if (existDot == null)
        //            {
        //                permissionsToAdd.Add(dot);
        //            }
        //        }
        //    }
        //}

        //if (permissionsToAdd.Any())
        //{
        //    foreach (var item in permissionsToAdd)
        //    {
        //        if (item.SuperiorId == 0)
        //        {
        //            item.SuperiorId = null;
        //        }
        //    }
        //    await _context.Permissions.AddRangeAsync(permissionsToAdd);
        //    await _context.SaveChangesAsync();
        //}

        // 获取类的类型
        Type type = typeof(User);
        // 获取类的所有属性
        PropertyInfo[] properties = type.GetProperties();

        // 遍历属性并输出它们的名称和类型
        foreach (PropertyInfo property in properties)
        {
            var a= property.PropertyType.Name;
            var d = type.CustomAttributes?
                .FirstOrDefault(x=>x.AttributeType.Name== "DescriptionAttribute")?
                .ConstructorArguments?
                .FirstOrDefault().Value;
            Console.WriteLine($"属性名称: {property.Name}, 属性类型: {property.PropertyType}");
        }

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            Console.WriteLine($"Field Name: {field.Name}, Field Type: {field.FieldType}");
        }

        GenerateCodeCQRS.GenerateControllerCode(type, "Application.Features");

        return 0;
    }

    /// <summary>
    /// 生成CQRS代码
    /// </summary>
    public static class GenerateCodeCQRS
    {
        public static bool GenerateAddCommandCode(Type type, string nameSpace)
        {
            var filePath = $@"D:\Add{type.Name}Command.cs";
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
            return true;
        }

        public static bool GenerateUpdateCommandCode(Type type, string nameSpace)
        {
            var filePath = $@"D:\Update{type.Name}Command.cs";
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
            return true;
        }

        public static bool GenerateDeleteCommandCode(Type type, string nameSpace)
        {
            var filePath = $@"D:\Delete{type.Name}Command.cs";
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
            return true;
        }

        public static bool GenerateControllerCode(Type type, string nameSpace)
        {
            var filePath = $@"D:\{type.Name}Controller.cs";
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
            return true;
        }
    }
}
