using Ardalis.Specification;
using Domain.Entities.Settings;

namespace Application.Features.UserProfileSettings.Specifications;

public class UserProfileSettingsByIdSpec : Specification<UserProfileSetting>
{
    public UserProfileSettingsByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
