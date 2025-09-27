using BlazorStatic;

using WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddBlazorStaticService(_ =>
{
    //opt. //check to change the defaults
}
).AddBlazorStaticContentService<BlogFrontMatter>();

builder.Services.AddRazorComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.UseBlazorStaticGenerator(!app.Environment.IsDevelopment());

app.Run();

internal static class WebsiteKeys
{
    public const string GitHubRepo = "https://github.com/BlazorStatic/WebApp";
    public const string X = "https://x.com/";
    public const string Title = "BlazorStatic Minimal Blog";
    public const string BlogPostStorageAddress = $"{GitHubRepo}/tree/main/Content/Blog";
    public const string BlogLead = "Sample blog created with BlazorStatic and TailwindCSS";
}
