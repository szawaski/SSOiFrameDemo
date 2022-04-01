using Microsoft.AspNetCore.Mvc;
using SharedSecurity;
using FrameApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Zerra.Web;

namespace FrameApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //Remove this section. This would normally read in the middleware pipeline
            //----------------------------------------------------------------
            var cm = new CookieManager(this.HttpContext);
            var userName = cm.Get("FrameAppAuthCookie");
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

                cm.Remove("FrameAppAuthCookie");
            }
            //----------------------------------------------------------------

            var data = Request.Query["data"];
            if (String.IsNullOrWhiteSpace(data))
                throw new Exception("Expected parameter data");

            //Read data sent from MainApp. Note this token expires after the time set in Transport.CreateSecureToken.
            var dataFromMainApp = Transport.ReadSecureToken<TestDataModel>(data);

            var user = Auth.GetUserFromClaims();
            if (user == null)
            {
                //recreate the token with longer expiration to allow the user time to login
                var newData = Transport.CreateSecureToken(dataFromMainApp, 300);
                return RedirectToAction("LoginFromFrameSSO", "Auth", new { returnUrl = Url.Action("Index", "Home", new { newData }) });
            }

            //Validate the user in the FrameApp is the same as the MainApp
            if (dataFromMainApp.User.Email != user.Email)
            {
                return RedirectToAction("LogoutFromFrameSSO", "Auth", new { returnUrl = Url.Action("Index", "Home", new { data }) });
            }

            var viewModel = new HomeViewModel
            {
                UserName = user.Email,
                Roles = user.Roles,
                DataFromMainApp = dataFromMainApp.Data
            };

            return View(viewModel);
        }
    }
}
