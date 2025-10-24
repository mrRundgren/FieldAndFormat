namespace WebApp.Extensions;

using Microsoft.AspNetCore.Components;

public static class HtmlContentExtensions
{
    public static MarkupString ToMarkupString(this string html) => (MarkupString)html;
}
