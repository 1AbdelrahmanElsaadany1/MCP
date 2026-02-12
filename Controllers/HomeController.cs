using Microsoft.AspNetCore.Mvc;

namespace MCPv1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
