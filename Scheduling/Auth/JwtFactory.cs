using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Scheduling.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly RoleManager<IdentityRole> _roleManager;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity, string siteName, string roleid, string privileges)
        {
            var UTCDateTime = DateTime.UtcNow;
            var Iat = ToUnixEpochDate(UTCDateTime).ToString();
            var claims = new[]
         {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, Iat, ClaimValueTypes.Integer64),
                 identity.FindFirst(ClaimTypes.PrimarySid),
                 identity.FindFirst(ClaimTypes.Role),
                 identity.FindFirst(ClaimTypes.Name),
                 new Claim(CustomClaimTypes.SiteName,siteName ),
                 new Claim(CustomClaimTypes.Permission,privileges ),
                 new Claim(CustomClaimTypes.RoleId,roleid )
             };

            //Get User Permission from Claims
            //var userPermissions = await GetUserPermissionsAsync(identity.FindFirst(ClaimTypes.Role).Value);
            // var userClaims = claims.Concat(userPermissions);

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: UTCDateTime,
                expires: UTCDateTime.Add(_jwtOptions.ValidFor),
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id, string role, string roleid, string siteName, string privileges)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                 new Claim(ClaimTypes.PrimarySid, id),
                 new Claim(ClaimTypes.Role, role),
                 new Claim(ClaimTypes.Name, userName),
                 new Claim(CustomClaimTypes.SiteName, siteName),
                 new Claim(CustomClaimTypes.RoleId, roleid),
                 new Claim(CustomClaimTypes.Permission, privileges)
            });
        }

        public async Task<List<Claim>> GetUserPermissionsAsync(string roleName)
        {
            //TODO: Get permissions from db
            IdentityRole identityRole = await _roleManager.FindByNameAsync(roleName);
            var permissions = await _roleManager.GetClaimsAsync(identityRole);
            return permissions.ToList();

        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }


    }
}
