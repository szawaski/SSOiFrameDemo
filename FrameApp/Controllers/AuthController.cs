using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedSecurity;
using FrameApp.Models;
using System;
using System.Threading.Tasks;
using Zerra.Identity;
using Zerra.Web;

namespace FrameApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult LoginFromFrameSSO(string returnUrl)
        {
            var viewModel = new LoginFromFrameSSOViewModel()
            {
                ReturnUrl = returnUrl,
            };

            return View(viewModel);
        }

        public IActionResult LogoutFromFrameSSO(string returnUrl)
        {
            var viewModel = new LogoutFromFrameSSOViewModel()
            {
                ReturnUrl = returnUrl,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> LoginSSO(string returnUrl)
        {
            var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

            var response = await identityConsumer.Login(returnUrl);

            return response.ToIActionResult();
        }

        public async Task<IActionResult> LoginCallbackSSO()
        {
            try
            {
                var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

                var request = new IdentityHttpRequest(this.HttpContext);
                var identity = await identityConsumer.LoginCallback(request);
                var returnUrl = identity.State;

                //Replace with local authentication mechanism
                //----------------------------------------------------------------
                var cm = new CookieManager(this.HttpContext);
                cm.Add("FrameAppAuthCookie", identity.UserName, null, SameSiteMode.None, true, true);
                //----------------------------------------------------------------

                if (!String.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                return View();
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        public async Task<IActionResult> LogoutSSO(string returnUrl)
        {
            var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

            var response = await identityConsumer.Logout(returnUrl);

            return response.ToIActionResult();
        }

        public async Task<IActionResult> LogoutCallbackSSO()
        {
            try
            {
                var identityConsumer = await SSOProvider.GetIdentityConsumerAsync();

                var request = new IdentityHttpRequest(this.HttpContext);
                var logout = await identityConsumer.LogoutCallback(request);
                var returnUrl = logout.State;

                //Replace with local authentication mechanism
                //----------------------------------------------------------------
                var cm = new CookieManager(this.HttpContext);
                cm.Remove("FrameAppAuthCookie", SameSiteMode.None, true, true);
                //----------------------------------------------------------------

                if (!String.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                return View();
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }
    }
}
