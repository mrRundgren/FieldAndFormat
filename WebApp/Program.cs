using BlazorStatic;
using WebApp;
using WebApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddBlazorStaticService()
    .AddBlazorStaticContentService<PageFrontMatter>(options =>
    {
        options.ContentPath = "content";
        options.PageUrl = "pages";
        options.Tags.TagsPageUrl = "tags";
    })
    .AddRazorComponents();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseAntiforgery();

app.MapRazorComponents<App>();
app.UseBlazorStaticGenerator(!app.Environment.IsDevelopment());
app.Run();
