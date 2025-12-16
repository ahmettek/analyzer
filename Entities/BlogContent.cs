namespace DotNet8.WebApi.Entities;

public class BlogContent
{
    public List<BlogContentItem> Items { get; set; } = [];
}

public class BlogContentItem
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

