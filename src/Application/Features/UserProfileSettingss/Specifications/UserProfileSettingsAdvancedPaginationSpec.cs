using Ardalis.Specification;
using Domain.Entities.Settings;


namespace Application.Features.UserProfileSettings.Specifications;

public class UserProfileSettingsAdvancedPaginationSpec : Specification<UserProfileSetting>
{
    public UserProfileSettingsAdvancedPaginationSpec(UserProfileSettingsAdvancedFilter filter)
    {
        Query     
            .Where(x => x.SettingName == filter.SettingName, !filter.SettingName.IsNullOrEmpty())
     
            .Where(x => x.SettingValue == filter.SettingValue, !filter.SettingValue.IsNullOrEmpty())
;    }
}