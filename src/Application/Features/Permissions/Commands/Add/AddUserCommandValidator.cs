namespace Application.Features.Permissions.Commands.Add
{
    public class AddPermissionCommandValidator : AbstractValidator<AddPermissionCommand>
    {
        private readonly IApplicationDbContext _context;
        public AddPermissionCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.SuperiorId)
                  .MustAsync(BeExistSuperiorId).WithMessage($"角色不存在");
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
    }
}
