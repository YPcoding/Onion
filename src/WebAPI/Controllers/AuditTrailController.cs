using Application.Features.AuditTrails.Queries.Pagination;
using Domain.Entities.Audit;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 审计日志
    /// </summary>
    public class AuditTrailController : ApiControllerBase
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        [HttpPost("PaginationQuery")]

        public async Task<Result<PaginatedData<AuditTrail>>> PaginationQuery(AuditTrailsWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
