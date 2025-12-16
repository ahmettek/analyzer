namespace DotNet8.WebApi.Models.ViewModels;

public class SitemapItem
{
    public DateTime? DateAdded { get; set; }
    public string URL { get; set; } = string.Empty;
    public string Priority { get; set; } = "0.5";
    public string? ChangeFreq { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageTitle { get; set; }
}

