using Common.CodeGenPro;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    /// <summary>
    ///代码生成
    /// </summary>
    public class GenerateCodeController : ApiControllerBase
    {
        /// <summary>
        /// 生成后端代码
        /// </summary>
        /// <param name="generateCode">参数</param>
        /// <returns></returns>
        [HttpPost("Backend")]
        public  Result<IEnumerable<string>> GenerateBackendCode(GenerateCode generateCode)
        {
            var filePaths = new List<string>();
            if (generateCode.Type == GenerateCodeType.Caching || generateCode.Type == GenerateCodeType.GenerateAll) 
            {
                filePaths.Add(GenerateCodeCQRS.GenerateCachingCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName), 
                    generateCode.NameSpace, 
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Add || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateAddCommandCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Update || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateUpdateCommandCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Delete || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateDeleteCommandCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.DTOs || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateDTOsCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.EventHandlers || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateEventHandlersCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.GetAll || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateQueriesGetAllCode(
                     GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.GetById || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateQueriesGetByIdCode(
                     GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Pagination || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateQueriesPaginationCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.AdvancedFilter || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsFilterCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.AdvancedPaginationSpec || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsPaginationSpecCode(
                     GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.ByIdSpec || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateSpecificationsByIdSpecCode(
                     GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Controller || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeCQRS.GenerateControllerCode(
                     GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }

            return Result<IEnumerable<string>>.Success(filePaths);
        }

        /// <summary>
        /// 生成前端代码
        /// </summary>
        /// <param name="generateCode">参数</param>
        /// <returns></returns>
        [HttpPost("Frontend")]
        public Result<IEnumerable<string>> GenerateFrontendCode(GenerateCode generateCode)
        {
            var filePaths = new List<string>();

            if (generateCode.Type == GenerateCodeType.Api || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateApiCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Hook || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateHookCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Update || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateRuleCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Delete || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateTypesCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.DTOs || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateFormCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }
            if (generateCode.Type == GenerateCodeType.Index || generateCode.Type == GenerateCodeType.GenerateAll)
            {
                filePaths.Add(GenerateCodeVue.GenerateIndexCode(
                    GenerateCodeCQRS.GetTypeByFullClassName(generateCode.FullClassName),
                    generateCode.NameSpace,
                    generateCode.SavePath));
            }

            return Result<IEnumerable<string>>.Success(filePaths);
        }

        /// <summary>
        /// 生成代码参数
        /// </summary>
        public class GenerateCode
        {
            /// <summary>
            /// 类名: 如 "Domain.Entities.Identity.User";
            /// </summary>
            [Required(ErrorMessage =$"缺少参数FullClassName")]
            public string FullClassName { get; set; }
            /// <summary>
            /// 命名空间: 如 "Application.Features";
            /// </summary>
            [Required(ErrorMessage = $"缺少参数NameSpace")]
            public string NameSpace { get; set; }
            /// <summary>
            /// 保存路径：如 "D:\\Programming\\Net\\Onion\\src\\Application\\Features";
            /// </summary>
            [Required(ErrorMessage = $"缺少参数SavePath")]
            public string SavePath { get; set; }
            /// <summary>
            /// 生成代码类型（枚举）
            /// </summary>
            [Required(ErrorMessage = $"缺少参数Type")]
            public GenerateCodeType Type { get; set;}
        }
    }
}
