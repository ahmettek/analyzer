using DotNet8.WebApi.Data;
using DotNet8.WebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Controllers;

/// <summary>
/// MSSQL'den PostgreSQL'e veri migration controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MigrationController(AppDbContext postgresDb, MssqlDbContext mssqlDb) : ControllerBase
{
    /// <summary>
    /// Tüm verileri MSSQL'den PostgreSQL'e migrate eder
    /// </summary>
    [HttpPost("migrate-all")]
    public async Task<IActionResult> MigrateAll(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, object>();

        try
        {
            // Blogs
            var blogsResult = await MigrateBlogs(cancellationToken);
            results["blogs"] = blogsResult;

            // Categories
            var categoriesResult = await MigrateCategories(cancellationToken);
            results["categories"] = categoriesResult;

            // Accounts
            var accountsResult = await MigrateAccounts(cancellationToken);
            results["accounts"] = accountsResult;

            // Podcasts
            var podcastsResult = await MigratePodcasts(cancellationToken);
            results["podcasts"] = podcastsResult;

            // Books
            var booksResult = await MigrateBooks(cancellationToken);
            results["books"] = booksResult;

            // Lessons
            var lessonsResult = await MigrateLessons(cancellationToken);
            results["lessons"] = lessonsResult;

            return Ok(new { success = true, message = "Migration tamamlandı", results });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message, results });
        }
    }

    /// <summary>
    /// Sadece Blog verilerini migrate eder
    /// </summary>
    [HttpPost("migrate-blogs")]
    public async Task<IActionResult> MigrateBlogsEndpoint(CancellationToken cancellationToken)
    {
        try
        {
            var result = await MigrateBlogs(cancellationToken);
            return Ok(new { success = true, result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Sadece Category verilerini migrate eder
    /// </summary>
    [HttpPost("migrate-categories")]
    public async Task<IActionResult> MigrateCategoriesEndpoint(CancellationToken cancellationToken)
    {
        try
        {
            var result = await MigrateCategories(cancellationToken);
            return Ok(new { success = true, result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Migration durumunu kontrol eder
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        try
        {
            var mssqlBlogs = await mssqlDb.Blogs.CountAsync(cancellationToken);
            var mssqlCategories = await mssqlDb.Categories.CountAsync(cancellationToken);
            var mssqlAccounts = await mssqlDb.Accounts.CountAsync(cancellationToken);

            var pgBlogs = await postgresDb.Blogs.CountAsync(cancellationToken);
            var pgCategories = await postgresDb.Categories.CountAsync(cancellationToken);
            var pgAccounts = await postgresDb.Accounts.CountAsync(cancellationToken);

            return Ok(new
            {
                mssql = new { blogs = mssqlBlogs, categories = mssqlCategories, accounts = mssqlAccounts },
                postgresql = new { blogs = pgBlogs, categories = pgCategories, accounts = pgAccounts },
                migrationNeeded = new
                {
                    blogs = mssqlBlogs > pgBlogs,
                    categories = mssqlCategories > pgCategories,
                    accounts = mssqlAccounts > pgAccounts
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private async Task<object> MigrateBlogs(CancellationToken cancellationToken)
    {
        var mssqlBlogs = await mssqlDb.Blogs.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Blogs.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newBlogs = mssqlBlogs.Where(b => !existingIds.Contains(b.Id)).ToList();
        
        // DateTime'ları UTC'ye çevir (PostgreSQL için gerekli)
        foreach (var blog in newBlogs)
        {
            blog.CreatedDate = DateTime.SpecifyKind(blog.CreatedDate, DateTimeKind.Utc);
            blog.UpdatedDate = DateTime.SpecifyKind(blog.UpdatedDate, DateTimeKind.Utc);
        }
        
        if (newBlogs.Count > 0)
        {
            postgresDb.Blogs.AddRange(newBlogs);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlBlogs.Count, migrated = newBlogs.Count, skipped = mssqlBlogs.Count - newBlogs.Count };
    }

    private async Task<object> MigrateCategories(CancellationToken cancellationToken)
    {
        var mssqlData = await mssqlDb.Categories.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Categories.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newData = mssqlData.Where(x => !existingIds.Contains(x.Id)).ToList();
        
        if (newData.Count > 0)
        {
            postgresDb.Categories.AddRange(newData);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlData.Count, migrated = newData.Count, skipped = mssqlData.Count - newData.Count };
    }

    private async Task<object> MigrateAccounts(CancellationToken cancellationToken)
    {
        var mssqlData = await mssqlDb.Accounts.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Accounts.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newData = mssqlData.Where(x => !existingIds.Contains(x.Id)).ToList();
        
        // NULL değerleri düzelt
        foreach (var item in newData)
        {
            item.Email ??= string.Empty;
            item.Password ??= string.Empty;
        }
        
        if (newData.Count > 0)
        {
            postgresDb.Accounts.AddRange(newData);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlData.Count, migrated = newData.Count, skipped = mssqlData.Count - newData.Count };
    }

    private async Task<object> MigratePodcasts(CancellationToken cancellationToken)
    {
        var mssqlData = await mssqlDb.Podcasts.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Podcasts.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newData = mssqlData.Where(x => !existingIds.Contains(x.Id)).ToList();
        
        // DateTime'ları UTC'ye çevir (PostgreSQL için gerekli)
        foreach (var item in newData)
        {
            item.CreatedDate = DateTime.SpecifyKind(item.CreatedDate, DateTimeKind.Utc);
        }
        
        if (newData.Count > 0)
        {
            postgresDb.Podcasts.AddRange(newData);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlData.Count, migrated = newData.Count, skipped = mssqlData.Count - newData.Count };
    }

    private async Task<object> MigrateBooks(CancellationToken cancellationToken)
    {
        var mssqlData = await mssqlDb.Books.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Books.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newData = mssqlData.Where(x => !existingIds.Contains(x.Id)).ToList();
        
        if (newData.Count > 0)
        {
            postgresDb.Books.AddRange(newData);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlData.Count, migrated = newData.Count, skipped = mssqlData.Count - newData.Count };
    }

    private async Task<object> MigrateLessons(CancellationToken cancellationToken)
    {
        var mssqlData = await mssqlDb.Lessons.AsNoTracking().ToListAsync(cancellationToken);
        var existingIds = await postgresDb.Lessons.Select(x => x.Id).ToListAsync(cancellationToken);
        
        var newData = mssqlData.Where(x => !existingIds.Contains(x.Id)).ToList();
        
        if (newData.Count > 0)
        {
            postgresDb.Lessons.AddRange(newData);
            await postgresDb.SaveChangesAsync(cancellationToken);
        }

        return new { total = mssqlData.Count, migrated = newData.Count, skipped = mssqlData.Count - newData.Count };
    }
}

