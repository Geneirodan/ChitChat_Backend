using Identity.Emails.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Identity.Emails.Services;

public class RazorViewRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider
) : IRazorViewRenderer
{
    private async Task<string> Render<TModel>(string viewName, TModel model)
    {
        var actionContext = GetActionContext();
        var view = FindView(actionContext, viewName);

        await using var output = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary()
            )
            {
                Model = model
            },
            new TempDataDictionary(
                context: actionContext.HttpContext,
                provider: tempDataProvider
            ),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext).ConfigureAwait(false);

        var result =  output.ToString();
        Console.WriteLine(result);
        return result;
    }

    public Task<string> RenderEmailConfirmationEmail(EmailConfirmationViewModel model) =>
        Render("Emails/EmailConfirmation", model);

    public Task<string> RenderForgotPasswordEmail(ForgotPasswordViewModel model) =>
        Render("Emails/ForgotPassword", model);

    private IView FindView(ActionContext actionContext, string viewName)
    {
        var getViewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success)
            return getViewResult.View;

        var findViewResult = viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success)
            return findViewResult.View;

        // nothing found, return an error
        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorLines =
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }
                .Concat(searchedLocations);

        throw new InvalidOperationException(string.Join(Environment.NewLine, errorLines));
    }

    private ActionContext GetActionContext()
    {
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        MapRoutes(actionContext);

        return actionContext;
    }

    private void MapRoutes(ActionContext actionContext)
    {
        var routes = new RouteBuilder(new ApplicationBuilder(serviceProvider))
        {
            DefaultHandler = new DefaultHandler()
        };
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}"
        );
        actionContext.RouteData.Routers.Add(routes.Build());
    }
    
    private class DefaultHandler : IRouter
    {
        public VirtualPathData? GetVirtualPath(VirtualPathContext context) => null;
        public Task RouteAsync(RouteContext context) => Task.CompletedTask;
    }
}