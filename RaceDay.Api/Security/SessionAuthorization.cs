using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SessionAuthorizeAttribute : TypeFilterAttribute
{
    public SessionAuthorizeAttribute(params string[] roles) : base(typeof(SessionAuthorizationFilter))
    {
        Arguments = new object[] { roles };
    }
}
public class SessionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly string[] _roles;

    public SessionAuthorizationFilter(string[] roles)
    {
        _roles = roles ?? Array.Empty<string>();
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");
        var role = session.GetString("Role");
        if (userId is null || string.IsNullOrWhiteSpace(role))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Please log in first." });
            return Task.CompletedTask;
        }
        if (_roles.Length > 0 && !_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ObjectResult(new { message = "You do not have permission to access this resource." }) { StatusCode = StatusCodes.Status403Forbidden };
        }
        return Task.CompletedTask;
    }
}
