namespace DotNet8.WebApi.Entities;

public class Lesson : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid? RootId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid BookId { get; set; }
    public int Order { get; set; }
}

