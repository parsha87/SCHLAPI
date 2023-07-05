using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Scheduling.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity, string siteName, string roleid, string privileges);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id, string role, string roleid, string SiteName, string privileges);

        Task<List<Claim>> GetUserPermissionsAsync(string roleName);
    }
}
