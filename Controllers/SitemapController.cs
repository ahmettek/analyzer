using System.Xml;
using DotNet8.WebApi.Data;
using DotNet8.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Controllers;

public class SitemapController(AppDbContext dbContext) : Controller
{
    private const string Website = "https://icerikfikri.com/";

    [HttpGet]
    [Route("/sitemap.xml")]
    [Route("/sitemap/getsitemap")]
    [Produces("application/xml")]
    [ResponseCache(Duration = 3600)] // 1 saat cache
    public async Task<IActionResult> GetSitemap(CancellationToken cancellationToken)
    {
        var contentList = await dbContext.Blogs
            .Where(x => x.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var sitemapItems = new List<SitemapItem>
        {
            // Ana sayfa - en yüksek öncelik
            new()
            {
                URL = "",
                Priority = "1.0",
                ChangeFreq = "daily",
                DateAdded = DateTime.Now
            }
        };

        // Blog içerikleri
        foreach (var content in contentList)
        {
            var daysSinceUpdate = (DateTime.Now - content.UpdatedDate).TotalDays;
            var priority = CalculatePriority(daysSinceUpdate);
            var changeFreq = CalculateChangeFreq(daysSinceUpdate);

            sitemapItems.Add(new SitemapItem
            {
                URL = content.Slug,
                Priority = priority,
                ChangeFreq = changeFreq,
                DateAdded = content.UpdatedDate,
                ImageUrl = content.ImageUrl != "/" ? content.ImageUrl : null,
                ImageTitle = content.SeoTitle
            });
        }

        var xml = GenerateSitemapXml(sitemapItems);
        return Content(xml, "application/xml");
    }

    /// <summary>
    /// İçerik yaşına göre priority hesaplar
    /// </summary>
    private static string CalculatePriority(double daysSinceUpdate) => daysSinceUpdate switch
    {
        <= 7 => "0.9",
        <= 30 => "0.8",
        <= 90 => "0.7",
        <= 180 => "0.6",
        <= 365 => "0.5",
        _ => "0.4"
    };

    /// <summary>
    /// İçerik yaşına göre changefreq hesaplar
    /// </summary>
    private static string CalculateChangeFreq(double daysSinceUpdate) => daysSinceUpdate switch
    {
        <= 1 => "hourly",
        <= 7 => "daily",
        <= 30 => "weekly",
        <= 365 => "monthly",
        _ => "yearly"
    };

    private string GenerateSitemapXml(List<SitemapItem> sitemapItems)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8
        };

        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
        writer.WriteAttributeString("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");

        foreach (var siteMapItem in sitemapItems)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", Website + siteMapItem.URL);

            if (siteMapItem.DateAdded.HasValue)
            {
                writer.WriteElementString("lastmod", siteMapItem.DateAdded.Value.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            }

            writer.WriteElementString("changefreq", siteMapItem.ChangeFreq ?? "weekly");
            writer.WriteElementString("priority", siteMapItem.Priority);

            if (!string.IsNullOrEmpty(siteMapItem.ImageUrl))
            {
                writer.WriteStartElement("image", "image", "http://www.google.com/schemas/sitemap-image/1.1");
                writer.WriteElementString("loc", "http://www.google.com/schemas/sitemap-image/1.1", siteMapItem.ImageUrl);
                if (!string.IsNullOrEmpty(siteMapItem.ImageTitle))
                {
                    writer.WriteElementString("title", "http://www.google.com/schemas/sitemap-image/1.1", siteMapItem.ImageTitle);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }
}

