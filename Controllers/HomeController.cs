using System.Diagnostics;
using DotNet8.WebApi.Data;
using DotNet8.WebApi.Entities;
using DotNet8.WebApi.Models.DTOs;
using DotNet8.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Controllers;

public class HomeController(AppDbContext dbContext) : Controller
{
    public async Task<ActionResult> Index(CancellationToken cancellationToken)
    {
        // Veritabanında limit uygula - sadece gerekli sayıda kayıt çek
        var blogList = await dbContext.Blogs
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .Take(12)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var categories = await dbContext.Categories
            .Take(10)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var categoryModel = categories.Select(x => new PodcastCategory.CategoryItem
        {
            Title = x.Name,
            Info = x.SeoDescription,
            Count = x.Count,
            ImageUrl = x.ImageUrl,
            Slug = x.Slug
        }).ToList();

        var topBlogItems = blogList.Take(2).Select(MapToBlogItemDto).ToList();
        var blogItems = blogList.Skip(2).Take(10).Select(MapToBlogItemDto).ToList();

        return View("Index", new HomePageViewModel
        {
            BlogList = blogItems,
            TopBlogList = topBlogItems,
            PodcastCategoryModel = new PodcastCategory { Categories = categoryModel }
        });
    }

    [Route("/robots.txt")]
    [ResponseCache(Duration = 86400)] // 1 gün cache
    public IActionResult RobotsText()
    {
        const string content = """
            # robots.txt for icerikfikri.com

            User-agent: *
            Allow: /
            Disallow: /admin/
            Disallow: /private/
            Disallow: /bblog/create
            Disallow: /bblog/edit
            Disallow: /bblog/list
            Disallow: /bblog/saveorupdate
            Disallow: /bblog/saveorupdatecontent
            Disallow: /bblog/sortcontent

            # Sitemap
            Sitemap: https://icerikfikri.com/sitemap/getsitemap

            # Crawl-delay
            Crawl-delay: 1

            # Host
            Host: https://icerikfikri.com
            """;
        return Content(content, "text/plain");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static BlogItemDto MapToBlogItemDto(Blog x) => new()
    {
        Content = x.Content,
        SeoDescription = x.SeoDescription,
        SeoTitle = x.SeoTitle,
        Id = x.Id,
        Slug = x.Slug,
        ImageUrl = x.ImageUrl,
        Author = x.Author,
        CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy")
    };
}

