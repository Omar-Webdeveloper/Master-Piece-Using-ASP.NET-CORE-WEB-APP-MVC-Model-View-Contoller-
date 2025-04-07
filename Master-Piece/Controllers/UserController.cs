using Microsoft.AspNetCore.Mvc;

namespace Master_Piece.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        public IActionResult BookedServicesHistory()
        {
            return View();
        }

    }
}
