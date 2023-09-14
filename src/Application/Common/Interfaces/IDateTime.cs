namespace Application.Common.Interfaces;

public interface IDateTime : IScopedDependency
{
    DateTime Now { get; }
}
