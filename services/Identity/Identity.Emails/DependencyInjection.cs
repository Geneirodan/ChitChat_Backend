using Identity.Emails.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Emails;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailRazorService(this IServiceCollection services)
    {
        services.AddMvcCore().AddRazorViewEngine();
        return services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
    }
}