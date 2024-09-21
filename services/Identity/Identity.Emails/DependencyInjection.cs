using Identity.Emails.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Emails;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailRazorService(this IServiceCollection services)
    {
        services.AddMvcCore().AddRazorViewEngine();
        // services.Configure<RazorViewEngineOptions>( options => {
        //     options.ViewLocationFormats.Add("/{1}/{0}.cshtml");
        //     options.ViewLocationFormats.Add("/Shared/{0}.cshtml");
        // });
        return services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
    }
}