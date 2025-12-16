using System.Globalization;
using System.Text.RegularExpressions;

namespace DotNet8.WebApi.Helpers;

/// <summary>
/// URL slug oluşturma yardımcı sınıfı
/// </summary>
public static partial class UrlHelper
{
    /// <summary>
    /// Verilen metni SEO-dostu URL slug'a çevirir
    /// </summary>
    public static string ToUrlSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // Küçük harfe çevir (Türkçe locale ile)
        value = value.ToLower(new CultureInfo("tr-TR"));
        
        // Türkçe karakterleri dönüştür
        value = value
            .Replace("ü", "u")
            .Replace("ö", "o")
            .Replace("ı", "i")
            .Replace("ş", "s")
            .Replace("ç", "c")
            .Replace("ğ", "g");
        
        // Boşlukları tire ile değiştir
        value = value.Replace(" ", "-");
        
        // Ardışık tire/alt çizgileri tek karaktere indir
        value = MultipleHyphensRegex().Replace(value, "$1");
        
        // Geçersiz karakterleri kaldır
        value = InvalidCharsRegex().Replace(value, "");
        
        // Baştan ve sondan tire/alt çizgi temizle
        value = value.Trim('-', '_');

        return value;
    }

    [GeneratedRegex(@"([-_]){2,}", RegexOptions.Compiled)]
    private static partial Regex MultipleHyphensRegex();

    [GeneratedRegex(@"[^a-z0-9\s\-_]", RegexOptions.Compiled)]
    private static partial Regex InvalidCharsRegex();
}

