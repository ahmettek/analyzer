namespace DotNet8.WebApi.Models.DTOs;

public class CategoryItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
    public int Count { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

