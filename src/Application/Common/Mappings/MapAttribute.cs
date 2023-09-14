namespace Application.Common.Mappings;

public class MapAttribute : Attribute
{
    public MapAttribute(params Type[] targetTypes)
    {
        TargetTypes = targetTypes;
    }
    public Type[] TargetTypes { get; }
}
