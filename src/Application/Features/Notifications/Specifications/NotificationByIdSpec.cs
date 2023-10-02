using Ardalis.Specification;
using Domain.Entities.Notifications;

namespace Application.Features.Notifications.Specifications;

public class NotificationByIdSpec : Specification<Notification>
{
    public NotificationByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
