using DotNet8.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Data;

/// <summary>
/// Ana DbContext - PostgreSQL (Railway)
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Podcast> Podcasts => Set<Podcast>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PostgreSQL için tablo isimleri (küçük harf, snake_case tercih edilir)
        modelBuilder.Entity<Account>().ToTable("account").HasKey(x => x.Id);
        modelBuilder.Entity<Blog>().ToTable("blog").HasKey(x => x.Id);
        modelBuilder.Entity<Category>().ToTable("category").HasKey(x => x.Id);
        modelBuilder.Entity<Podcast>().ToTable("podcast").HasKey(x => x.Id);
        modelBuilder.Entity<Book>().ToTable("book").HasKey(x => x.Id);
        modelBuilder.Entity<Lesson>().ToTable("lesson").HasKey(x => x.Id);

        base.OnModelCreating(modelBuilder);
    }
}

/// <summary>
/// Migration için MSSQL DbContext (sadece okuma amaçlı)
/// </summary>
public class MssqlDbContext(DbContextOptions<MssqlDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Podcast> Podcasts => Set<Podcast>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MSSQL'deki mevcut tablo isimleri (tekil)
        modelBuilder.Entity<Account>().ToTable("Account").HasKey(x => x.Id);
        modelBuilder.Entity<Blog>().ToTable("Blog").HasKey(x => x.Id);
        modelBuilder.Entity<Category>().ToTable("Category").HasKey(x => x.Id);
        modelBuilder.Entity<Podcast>().ToTable("Podcast").HasKey(x => x.Id);
        modelBuilder.Entity<Book>().ToTable("Book").HasKey(x => x.Id);
        modelBuilder.Entity<Lesson>().ToTable("Lesson").HasKey(x => x.Id);

        base.OnModelCreating(modelBuilder);
    }
}

