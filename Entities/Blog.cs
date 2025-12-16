namespace DotNet8.WebApi.Entities;

public class Blog : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedDate { get; set; }
}

