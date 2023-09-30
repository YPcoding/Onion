using Application.Features.GenerateCodes.Commands.Backend;
using Application.Features.GenerateCodes.Commands.Frontend;
using Application.Features.GenerateCodes.DTOs;
using Application.Features.GenerateCodes.Queries.GetPath;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers
{
    /// <summary>
    ///代码生成
    /// </summary>
    [Description("代码生成器")]
    public class CodeController : ApiControllerBase
    {
        /// <summary>
        /// 查询存放路径
        /// </summary>
        /// <returns></returns>
        [HttpPost("Query/Save/Path")]

        public async Task<Result<GenerateCodeDto>> PathQuery()
        {
            return await Mediator.Send(new GetCodeSavePathQuery());
        }

        /// <summary>
        /// 生成后端代码
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns></returns>
        [HttpPost("Backend")]
        public async Task<Result<IEnumerable<string>>> GenerateBackendCodeAsync(GenerateBackendCodeCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// 生成前端代码
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns></returns>
        [HttpPost("Frontend")]
        public async Task<Result<IEnumerable<string>>> GenerateFrontendCodeAsync(GenerateFrontendCodeCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
