using Application.Common.Extensions;
using Application.Features.Chats.DTOs;
using Application.Features.Chats.Specifications;
using Domain.Repositories;

namespace Application.Features.Chats.Queries;


/// <summary>
/// 聊天用户分页查询
/// </summary>
[Description("聊天用户分页查询")]
public class ChatUsersWithPaginationQuery : ChatUserAdvancedFilter, IRequest<Result<PaginatedData<ChatUserDto>>>
{
    public override string ToString()
    {
        return
            $"Search:{Keyword},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
    [JsonIgnore]
    public ChatUserAdvancedPaginationSpec Specification => new ChatUserAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class ChatUsersWithPaginationQueryHandler :
IRequestHandler<ChatUsersWithPaginationQuery, Result<PaginatedData<ChatUserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ChatUsersWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IRoleRepository roleRepository)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回用户分页数据</returns>
    public async Task<Result<PaginatedData<ChatUserDto>>> Handle(
        ChatUsersWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<User, ChatUserDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<ChatUserDto>>.SuccessAsync(users);
    }
}
