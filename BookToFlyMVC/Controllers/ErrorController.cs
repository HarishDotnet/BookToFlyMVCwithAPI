using Microsoft.AspNetCore.Mvc;

namespace BookToFlyMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Return a generic error view
        }
    }

}
