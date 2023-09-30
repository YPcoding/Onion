using Application.Features.Permissions.Commands.Add;
using Application.Features.Permissions.Commands.Delete;
using Application.Features.Permissions.Commands.Update;
using Application.Features.Permissions.DTOs;
using Application.Features.Permissions.Queries.GetAll;
using Application.Features.Permissions.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public class PermissionController : ApiControllerBase
    {
        /// <summary>
        /// 全部查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("Query/All")]

        public async Task<Result<IEnumerable<PermissionDto>>> GetAll()
        {
            return await Mediator.Send(new GetAllPermissionsQuery());
        }

        /// <summary>
        /// 单个查询
        /// </summary>
        /// <param name="permissionId">权限唯一标识</param>
        /// <returns></returns>
        [HttpGet("Query/{permissionId}")]

        public async Task<Result<PermissionDto>> GetByIdQuery(long permissionId)
        {
            return await Mediator.Send(new GetPermissionByIdQuery { PermissionId = permissionId });
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]

        public async Task<Result<long>> Add(AddPermissionCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <returns></returns>
        [HttpPut("Update")]

        public async Task<Result<long>> Update(UpdatePermissionCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Delete")]

        public async Task<Result<bool>> Delete(DeletePermissionCommand command)
        {
            return await Mediator.Send(command);
        }
        [HttpPost("Sync/API")]
    }
}
