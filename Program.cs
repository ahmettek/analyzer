using System.IO.Compression;
using DotNet8.WebApi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Add API documentation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL Database Context (Ana veritabanı - Railway)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// MSSQL Database Context (Migration için - sadece okuma)
builder.Services.AddDbContext<MssqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSql")));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(3);
    });

// Response Compression - Brotli and Gzip
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        "application/javascript",
        "application/json",
        "application/xml",
        "text/css",
        "text/html",
        "text/json",
        "text/plain",
        "text/xml",
        "image/svg+xml"
    ]);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// Response Caching
builder.Services.AddResponseCaching();

var app = builder.Build();

// PostgreSQL tablolarını oluştur (ilk çalıştırmada)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger and Scalar API documentation (available in all environments)
app.UseSwagger(opt => opt.RouteTemplate = "openapi/{documentName}.json");
app.MapScalarApiReference(opt =>
{
    opt.Title = "İçerik Fikri API";
    opt.Theme = ScalarTheme.BluePlanet;
    opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
});

app.UseHttpsRedirection();

// Response Compression Middleware - must be early in the pipeline
app.UseResponseCompression();

// Static Files with Caching Headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.File.Name.ToLowerInvariant();
        
        // CSS, JS, images: 1 year cache
        if (path.EndsWith(".css") || path.EndsWith(".js") ||
            path.EndsWith(".png") || path.EndsWith(".jpg") ||
            path.EndsWith(".jpeg") || path.EndsWith(".webp") ||
            path.EndsWith(".svg") || path.EndsWith(".ico") ||
            path.EndsWith(".woff") || path.EndsWith(".woff2") ||
            path.EndsWith(".ttf"))
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
        }
        else
        {
            // Other files: 1 day cache
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
        }
    }
});

// Response Caching Middleware
app.UseResponseCaching();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// MVC Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "content",
    pattern: "{slug}",
    defaults: new { controller = "Blog", action = "Detail" });

app.MapControllerRoute(
    name: "leveltest",
    pattern: "/online-test/ucretsiz-ingilizce-seviye-tespit-sinavi",
    defaults: new { controller = "Other", action = "LevelTest" });

app.MapControllerRoute(
    name: "lesson",
    pattern: "/blog/{slug}",
    defaults: new { controller = "Blog", action = "OldDetailRedirectNewDetail" });

app.Run();
