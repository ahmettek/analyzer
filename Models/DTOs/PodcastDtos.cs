namespace DotNet8.WebApi.Models.DTOs;

public class PodcastModel
{
    public string Title { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class PodcastCategory
{
    public List<CategoryItem> Categories { get; set; } = [];

    public class CategoryItem
    {
        public string Title { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

public class PodcastPageModel
{
    public PodcastCategory PodcastCategoryModel { get; set; } = new();
    public List<PodcastModel> PopulerPodcasts { get; set; } = [];
}

public class PodcastDetailPageResponseDto : ResponseBase
{
    public string Title { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class PodcastCategoryPageModel
{
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<PodcastModel> Podcasts { get; set; } = [];
}

