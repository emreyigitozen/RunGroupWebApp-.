using Microsoft.AspNetCore.Mvc;

namespace RunGroupWebApp.Controllers
{
    public class UserController : Controller
    {
        [HttpGet("users")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
