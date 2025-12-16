using System.Text;
using System.Text.Json;
using DotNet8.WebApi.Data;
using DotNet8.WebApi.Entities;
using DotNet8.WebApi.Helpers;
using DotNet8.WebApi.Models.DTOs;
using DotNet8.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Controllers;

public class BlogController(AppDbContext dbContext) : Controller
{
    [HttpPost]
    public async Task<ActionResult> SaveOrUpdate(BlogRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                SeoTitle = request.SeoTitle,
                SeoDescription = request.SeoDescription,
                Content = request.Content,
                Slug = UrlHelper.ToUrlSlug(request.SeoTitle),
                ImageUrl = "/",
                IsActive = true,
                Author = "icerikfikri",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                HtmlContent = ""
            };

            dbContext.Blogs.Add(blog);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Json(new ResponseBase { Success = true, Message = $"Id {blog.Id}" });
        }

        var item = await dbContext.Blogs.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item == null)
            return Json(new ResponseBase { Success = false, Message = "Blog bulunamadı" });

        item.Content = request.Content;
        item.SeoDescription = request.SeoDescription;
        item.SeoTitle = request.SeoTitle;
        item.UpdatedDate = DateTime.Now;
        item.CreatedDate = DateTime.Now;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Json(new ResponseBase { Success = true, Message = $"Id {item.Id}" });
    }

    [HttpPost]
    public async Task<ActionResult> SaveOrUpdateContent(BlogContentRequestDto request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Blogs.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item == null)
            return RedirectToAction("Edit", new { id = request.Id });

        item.UpdatedDate = DateTime.Now;

        try
        {
            var contents = JsonSerializer.Deserialize<BlogContent>(item.Content) ?? new BlogContent();
            var contentEleman = contents.Items.FirstOrDefault(x => x.Id == request.ContentId);

            if (contentEleman == null)
            {
                var lastItem = contents.Items.OrderByDescending(x => x.Id).FirstOrDefault();
                contents.Items.Add(new BlogContentItem
                {
                    Content = request.Content,
                    Type = request.Type,
                    Id = lastItem != null ? lastItem.Id + 1 : 1
                });

                item.Content = JsonSerializer.Serialize(contents);
                item.HtmlContent = GenerateHtmlContent(contents);
                await dbContext.SaveChangesAsync(cancellationToken);
                return RedirectToAction("Edit", new { id = item.Id });
            }

            var existingItem = contents.Items.FirstOrDefault(x => x.Id == request.ContentId);
            if (existingItem != null)
            {
                contents.Items.Remove(existingItem);
                contents.Items.Add(new BlogContentItem
                {
                    Content = request.Content,
                    Type = request.Type,
                    Id = request.ContentId
                });

                item.Content = JsonSerializer.Serialize(contents);
                item.HtmlContent = GenerateHtmlContent(contents);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch
        {
            item.Content = JsonSerializer.Serialize(new BlogContent());
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return RedirectToAction("Edit", new { id = item.Id });
    }

    [HttpPost]
    public async Task<ActionResult> SortContent(BlogSortContentRequestDto request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Blogs.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item == null)
            return RedirectToAction("Edit", new { id = request.Id });

        item.UpdatedDate = DateTime.Now;

        try
        {
            var blogContent = new BlogContent();
            var sort = 1;
            foreach (var contentsItem in request.SortedData)
            {
                blogContent.Items.Add(new BlogContentItem
                {
                    Content = contentsItem.Text,
                    Type = contentsItem.Type,
                    Id = sort++
                });
            }

            item.Content = JsonSerializer.Serialize(blogContent);
            item.HtmlContent = GenerateHtmlContent(blogContent);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error if needed
        }

        return RedirectToAction("Edit", new { id = item.Id });
    }

    [HttpGet]
    public async Task<ActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        var blog = await dbContext.Blogs
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Slug == slug, cancellationToken);

        if (blog != null)
        {
            var response = new BlogResponseDto
            {
                SeoTitle = blog.SeoTitle,
                SeoDescription = blog.SeoDescription,
                Content = blog.Content,
                HtmlContent = blog.HtmlContent,
                Slug = blog.Slug,
                Success = true,
                ImageUrl = blog.ImageUrl,
                Author = blog.Author,
                CreatedDate = blog.CreatedDate,
                ModifiedDate = blog.UpdatedDate
            };
            return View("Detail", response);
        }

        return View("Detail", new BlogResponseDto());
    }

    public ActionResult OldDetailRedirectNewDetail(string slug)
    {
        var newUrl = "https://www.icerikfikri.com/" + slug;
        return RedirectPermanent(newUrl);
    }

    [HttpGet]
    public async Task<ActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var blog = await dbContext.Blogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (blog != null)
        {
            var contentItemList = new BlogContent();
            try
            {
                contentItemList = JsonSerializer.Deserialize<BlogContent>(blog.Content) ?? new BlogContent();
            }
            catch
            {
                // Ignore deserialization errors
            }

            return View("Edit", new BlogItemDto
            {
                Content = blog.Content,
                SeoDescription = blog.SeoDescription,
                SeoTitle = blog.SeoTitle,
                Id = blog.Id,
                ContentItemList = contentItemList
            });
        }

        return View("Edit", new BlogItemDto());
    }

    [HttpGet]
    public async Task<ActionResult> List(CancellationToken cancellationToken)
    {
        var blogList = await dbContext.Blogs
            .OrderByDescending(x => x.CreatedDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return View("List", new BlogListViewModel
        {
            BlogList = blogList.Select(x => new BlogItemDto
            {
                Content = x.Content,
                SeoDescription = x.SeoDescription,
                SeoTitle = x.SeoTitle,
                Id = x.Id
            }).ToList()
        });
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View("Create");
    }

    private static string GenerateHtmlContent(BlogContent blogContent)
    {
        var sb = new StringBuilder();

        foreach (var item in blogContent.Items.OrderBy(x => x.Id))
        {
            switch (item.Type)
            {
                case "paragraph":
                    sb.AppendFormat("<p>{0}</p>", item.Content);
                    break;
                case "h2":
                    sb.AppendFormat("<h2>{0}</h2>", item.Content);
                    break;
                case "h3":
                    sb.AppendFormat("<h3>{0}</h3>", item.Content);
                    break;
                case "b":
                    sb.AppendFormat("<strong>{0}</strong>", item.Content);
                    break;
                case "a":
                    var pipeCount = item.Content.Count(c => c == '|');
                    if (pipeCount == 2)
                    {
                        var ahref = item.Content.Split('|');
                        sb.AppendFormat("<a class='in-content' href='{0}'>{1}</a>", ahref[1], ahref[2]);
                    }
                    break;
                case "li":
                    var pairs = item.Content.Split(['*'], StringSplitOptions.RemoveEmptyEntries);
                    var liBuilder = new StringBuilder();

                    foreach (var pair in pairs)
                    {
                        var parts = pair.Split('|');
                        if (parts.Length == 2)
                        {
                            var word = parts[0].Trim();
                            var meaning = parts[1].Trim();
                            liBuilder.AppendLine($"<li>{word} - <span class=\"meaning\">{meaning}</span></li>");
                        }
                    }

                    sb.Append($"<ul class=\"word-list\">{liBuilder}</ul>");
                    break;
            }
        }

        return sb.ToString();
    }
}

