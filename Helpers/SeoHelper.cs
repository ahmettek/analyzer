namespace DotNet8.WebApi.Helpers;

/// <summary>
/// SEO yardımcı metodları - Canonical URL, Open Graph, Twitter Card ve Schema.org üretimi
/// </summary>
public static class SeoHelper
{
    private const string BaseUrl = "https://icerikfikri.com";
    private const string SiteName = "İçerik Fikri";
    private const string DefaultImage = "https://icerikfikri.com/image/logo.png";
    private const string TwitterHandle = "@icerikfikri";

    /// <summary>
    /// Canonical URL üretir
    /// </summary>
    public static string GetCanonicalUrl(string? slug = null)
    {
        if (string.IsNullOrEmpty(slug))
            return BaseUrl;

        return $"{BaseUrl}/{slug.TrimStart('/')}";
    }

    /// <summary>
    /// Open Graph meta taglarını HTML olarak üretir
    /// </summary>
    public static string GenerateOpenGraphTags(
        string title,
        string description,
        string? slug = null,
        string? imageUrl = null,
        string type = "website")
    {
        var url = GetCanonicalUrl(slug);
        var image = string.IsNullOrEmpty(imageUrl) || imageUrl == "/" ? DefaultImage : imageUrl;

        return $"""
            <meta property="og:type" content="{type}" />
            <meta property="og:url" content="{url}" />
            <meta property="og:title" content="{EscapeHtml(title)}" />
            <meta property="og:description" content="{EscapeHtml(description)}" />
            <meta property="og:image" content="{image}" />
            <meta property="og:site_name" content="{SiteName}" />
            <meta property="og:locale" content="tr_TR" />
        """;
    }

    /// <summary>
    /// Twitter Card meta taglarını HTML olarak üretir
    /// </summary>
    public static string GenerateTwitterCardTags(
        string title,
        string description,
        string? imageUrl = null)
    {
        var image = string.IsNullOrEmpty(imageUrl) || imageUrl == "/" ? DefaultImage : imageUrl;

        return $"""
            <meta name="twitter:card" content="summary_large_image" />
            <meta name="twitter:site" content="{TwitterHandle}" />
            <meta name="twitter:title" content="{EscapeHtml(title)}" />
            <meta name="twitter:description" content="{EscapeHtml(description)}" />
            <meta name="twitter:image" content="{image}" />
        """;
    }

    /// <summary>
    /// Canonical link tag'i üretir
    /// </summary>
    public static string GenerateCanonicalTag(string? slug = null)
    {
        var url = GetCanonicalUrl(slug);
        return $"""<link rel="canonical" href="{url}" />""";
    }

    /// <summary>
    /// BreadcrumbList JSON-LD üretir
    /// </summary>
    public static string GenerateBreadcrumbSchema(string pageTitle, string? slug = null)
    {
        var pageUrl = GetCanonicalUrl(slug);

        return $$"""
        <script type="application/ld+json">
        {
          "@context": "https://schema.org",
          "@type": "BreadcrumbList",
          "itemListElement": [
            {
              "@type": "ListItem",
              "position": 1,
              "name": "Ana Sayfa",
              "item": "{{BaseUrl}}"
            },
            {
              "@type": "ListItem",
              "position": 2,
              "name": "{{EscapeJson(pageTitle)}}",
              "item": "{{pageUrl}}"
            }
          ]
        }
        </script>
        """;
    }

    /// <summary>
    /// WebSite JSON-LD üretir (ana sayfa için)
    /// </summary>
    public static string GenerateWebsiteSchema()
    {
        return $$"""
        <script type="application/ld+json">
        {
          "@context": "https://schema.org",
          "@type": "WebSite",
          "name": "{{SiteName}}",
          "url": "{{BaseUrl}}",
          "potentialAction": {
            "@type": "SearchAction",
            "target": "{{BaseUrl}}/search?q={search_term_string}",
            "query-input": "required name=search_term_string"
          }
        }
        </script>
        """;
    }

    /// <summary>
    /// Organization JSON-LD üretir
    /// </summary>
    public static string GenerateOrganizationSchema()
    {
        return $$"""
        <script type="application/ld+json">
        {
          "@context": "https://schema.org",
          "@type": "Organization",
          "name": "{{SiteName}}",
          "url": "{{BaseUrl}}",
          "logo": "{{DefaultImage}}",
          "sameAs": []
        }
        </script>
        """;
    }

    /// <summary>
    /// BlogPosting JSON-LD üretir
    /// </summary>
    public static string GenerateBlogPostingSchema(
        string title,
        string description,
        string slug,
        string author,
        DateTime createdDate,
        DateTime modifiedDate,
        string? imageUrl = null,
        string? articleBody = null)
    {
        var image = string.IsNullOrEmpty(imageUrl) || imageUrl == "/" ? DefaultImage : imageUrl;
        var body = string.IsNullOrEmpty(articleBody) ? description : articleBody;

        return $$"""
        <script type="application/ld+json">
        {
          "@context": "https://schema.org",
          "@type": "BlogPosting",
          "headline": "{{EscapeJson(title)}}",
          "description": "{{EscapeJson(description)}}",
          "image": "{{image}}",
          "datePublished": "{{createdDate:yyyy-MM-dd}}",
          "dateModified": "{{modifiedDate:yyyy-MM-dd}}",
          "author": {
            "@type": "Person",
            "name": "{{EscapeJson(author)}}"
          },
          "publisher": {
            "@type": "Organization",
            "name": "{{SiteName}}",
            "logo": {
              "@type": "ImageObject",
              "url": "{{DefaultImage}}"
            }
          },
          "mainEntityOfPage": {
            "@type": "WebPage",
            "@id": "{{GetCanonicalUrl(slug)}}"
          },
          "articleBody": "{{EscapeJson(body)}}"
        }
        </script>
        """;
    }

    /// <summary>
    /// Preconnect ve DNS-prefetch taglarını üretir
    /// </summary>
    public static string GeneratePreconnectTags()
    {
        return """<link rel="dns-prefetch" href="https://www.googletagmanager.com" />""";
    }

    /// <summary>
    /// HTML özel karakterlerini escape eder
    /// </summary>
    private static string EscapeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    /// <summary>
    /// JSON özel karakterlerini escape eder
    /// </summary>
    private static string EscapeJson(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}

