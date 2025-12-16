using DotNet8.WebApi.Entities;

namespace DotNet8.WebApi.Models.DTOs;

public class BlogRequestDto
{
    public Guid Id { get; set; }
    public int ContentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class BlogContentRequestDto
{
    public Guid Id { get; set; }
    public int ContentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class BlogSortContentRequestDto
{
    public Guid Id { get; set; }
    public List<SortedItem> SortedData { get; set; } = [];

    public class SortedItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}

public class BlogResponseDto : ResponseBase
{
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class BlogItemDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public BlogContent? ContentItemList { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
}

public class BlogSaveDto
{
    public string Content { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
}

public class BlogUpdateDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
}

