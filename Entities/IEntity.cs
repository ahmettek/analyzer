namespace DotNet8.WebApi.Entities;

public interface IEntity<TId>
{
    TId Id { get; set; }
}

