using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Permissions.Commands.Add
{
    public class AddPermissionCommandValidator : AbstractValidator<AddPermissionCommand>
    {
        private readonly IApplicationDbContext _context;
        public AddPermissionCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.SuperiorId)
                  .MustAsync(BeExistSuperiorId).WithMessage($"上级不存在");

            RuleFor(v => v.SuperiorId)
                  .MustAsync(BeValidSuperiorId).WithMessage($"上级节点不能是权限点类型");
        }



        /// <summary>
        /// 校验上级权限是否存在
        /// </summary>
        /// <param name="superiorId">上级权限唯一标识</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        private async Task<bool> BeExistSuperiorId(long? superiorId, CancellationToken cancellationToken)
        {
            if (superiorId.HasValue) return await _context.Permissions.AnyAsync(x => x.Id == superiorId);

            return true;
        }

        /// <summary>
        /// 校验上级节点是否合法
        /// </summary>
        /// <param name="superiorId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> BeValidSuperiorId(long? superiorId, CancellationToken cancellationToken)
        {
            if (superiorId.HasValue)
            {
                var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == superiorId);
                return !(permission?.Type == PermissionType.Dot);
            }

            return true;
        }
    }
}
