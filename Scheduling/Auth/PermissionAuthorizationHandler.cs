using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Auth
{
    /// <summary>
    /// PermissionRequirement authorization handler
    /// </summary>
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionAuthorizationHandler()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            var requiredPermissions = requirement.Permission.Split(',');

            //does user have any of the required permissions?
            var hasPermission = context.User.Claims.Any(x => x.Type == CustomClaimTypes.Permission &&
                                                       requiredPermissions.Any(permission => permission.Trim() == x.Value) &&
                                                       x.Issuer == "JISL_API");
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
