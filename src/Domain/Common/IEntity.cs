using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

public interface IEntity
{

}
public interface IEntity<T> : IEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    T Id { get; set; }
}
