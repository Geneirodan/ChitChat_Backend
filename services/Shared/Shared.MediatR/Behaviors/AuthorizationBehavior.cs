using System.Reflection;
using Ardalis.Result;
using Shared.Abstractions;
using Shared.MediatR.Attributes;

namespace Shared.MediatR.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(IUser user) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToArray();

        if (authorizeAttributes.Length == 0)
            return await next().ConfigureAwait(false);

        if (user is { Id: null })
            return Result<TResponse>.Unauthorized();
        
        var authorizeAttributesWithRoles =
            authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles)).ToArray();

        if (authorizeAttributesWithRoles.Length == 0)
            return await next().ConfigureAwait(false);
        
        var authorized = authorizeAttributesWithRoles.SelectMany(a => a.Roles.Split(','))
            .Any(role => user.IsInRole(role.Trim()));

        return authorized 
            ? await next().ConfigureAwait(false) 
            : Result<TResponse>.Forbidden();
    }
}