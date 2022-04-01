using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedSecurity;
using MainApp.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Zerra.Identity;
using Zerra.Web;
using System;

namespace MainApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            //Remove this section. This would normally read in the middleware pipeline
            //----------------------------------------------------------------
            var cm = new CookieManager(this.HttpContext);
            var userName = cm.Get("MainAppAuthCookie");
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
                claims.Add(new Claim(ClaimTypes.Name, userName));
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                var identity = new ClaimsIdentity(claims, "Cookie");
                var principal = new ClaimsPrincipal(identity);
                System.Threading.Thread.CurrentPrincipal = principal;
                this.HttpContext.User = principal;

                cm.Remove("MainAppAuthCookie");
            }
            //----------------------------------------------------------------

            var viewModel = new AuthViewModel();

            var user = Auth.GetUserFromClaims();
            if (user != null)
            {
                //Create token to send data to the FrameApp. Token expires in 90s.
                var modelToSend = new TestDataModel()
                {
                    Value1 = "stuff",
                    Value2 = 12345
                };
                var dataToSend = Transport.CreateSecureToken(modelToSend, 90);

                viewModel.UserName = user.Email;
                viewModel.Roles = user.Roles;
                viewModel.DataToSend = dataToSend;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> LoginSSO()
        {
            var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();
            var response = await identityConsumer.Login(null);
            return response.ToIActionResult();
        }

        public async Task<IActionResult> LoginCallbackSSO()
        {
            try
            {
                var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

                var request = new IdentityHttpRequest(this.HttpContext);
                var identity = await identityConsumer.LoginCallback(request);

                //Replace with local authentication mechanism
                //----------------------------------------------------------------
                var cm = new CookieManager(this.HttpContext);
                cm.Add("MainAppAuthCookie", identity.UserName, null, SameSiteMode.None, true, true);
                //----------------------------------------------------------------

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        public async Task<IActionResult> LogoutSSO()
        {
            var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

            var response = await identityConsumer.Logout(null);
            return response.ToIActionResult();
        }

        public async Task<IActionResult> LogoutCallbackSSO()
        {
            try
            {
                var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

                var request = new IdentityHttpRequest(this.HttpContext);
                _ = await identityConsumer.LogoutCallback(request);

                //Replace with local authentication mechanism
                //----------------------------------------------------------------
                var cm = new CookieManager(this.HttpContext);
                cm.Remove("MainAppAuthCookie", SameSiteMode.None, true, true);
                //----------------------------------------------------------------

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }
    }
}
