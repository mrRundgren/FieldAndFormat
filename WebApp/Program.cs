using System.IO.Compression;
using BlazorStatic;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using WebApp;
using WebApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    })
    .AddWebOptimizer(pipeline =>
    {
        pipeline.MinifyCssFiles("css/**/*.css");
    })
    .AddBlazorStaticService()
    .AddBlazorStaticContentService<PageFrontMatter>(options =>
    {
        options.ContentPath = "content";
        options.PageUrl = "pages";
        options.Tags.TagsPageUrl = "tags";
    })
    .AddRazorComponents();

builder.Services
    .Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    })
    .Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app
    .UseResponseCompression()
    .UseWebOptimizer()
    .UseStaticFiles(new StaticFileOptions
    {
        // FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "output")),
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=604800");
        }
    })
    .UseAntiforgery();

app.MapRazorComponents<App>();
app.UseBlazorStaticGenerator(true);
app.Run();
