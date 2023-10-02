using Ardalis.Specification;
using Domain.Entities.Notifications;


namespace Application.Features.Notifications.Specifications;

public class NotificationAdvancedPaginationSpec : Specification<Notification>
{
    public NotificationAdvancedPaginationSpec(NotificationAdvancedFilter filter)
    {
        Query     
            .Where(x => x.Title == filter.Title, !filter.Title.IsNullOrEmpty())
     
            .Where(x => x.Content == filter.Content, !filter.Content.IsNullOrEmpty())
     
            .Where(x => x.Link == filter.Link, !filter.Link.IsNullOrEmpty())
;    }
}