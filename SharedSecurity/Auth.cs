using System;
using System.Linq;
using System.Security.Claims;

namespace SharedSecurity
{
    public static class Auth
    {
        public static UserModel GetUserFromClaims()
        {
            var principal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal == null)
                return null;

            var user = new UserModel()
            {
                Email = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                Roles = principal.Claims.Where(x => x.Type == ClaimTypes.Role && !String.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value).ToArray()
            };

            return user;
        }
    }
}
