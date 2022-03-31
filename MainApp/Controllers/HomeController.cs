using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

