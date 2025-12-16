using DotNet8.WebApi.Models.DTOs;

namespace DotNet8.WebApi.Models.ViewModels;

public class HomePageViewModel
{
    public List<BlogItemDto> TopBlogList { get; set; } = [];
    public List<BlogItemDto> BlogList { get; set; } = [];
    public List<CategoryItemDto> Categories { get; set; } = [];
}

public class BlogListViewModel
{
    public List<BlogItemDto> BlogList { get; set; } = [];
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
