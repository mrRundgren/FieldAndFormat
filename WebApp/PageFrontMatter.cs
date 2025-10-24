// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace WebApp;

using BlazorStatic;

public class PageFrontMatter : IFrontMatter, IFrontMatterWithTags
{
    public string Title { get; set; } = "Unknown";
    public DateTime Created { get; set; } = DateTime.Now;
    public bool IsMenuItem { get; set; }
    public int Order { get; set; } = -1;
    public Dictionary<string, string>? Hero { get; set; }
    public List<string> Tags { get; set; } = [];
}
