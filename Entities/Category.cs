namespace DotNet8.WebApi.Entities;

public class Category : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public int Count { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

