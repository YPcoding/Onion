using Ardalis.Specification;

namespace Application.Features.Chats.Specifications;

public class ChatUserAdvancedPaginationSpec : Specification<User>
{
    public ChatUserAdvancedPaginationSpec(ChatUserAdvancedFilter filter)
    {
        Query.Where(x => x.UserName!.Contains(filter.Keyword!) || x.Realname!.Contains(filter.Keyword!), !filter.Keyword!.IsNullOrWhiteSpace());
    }
}