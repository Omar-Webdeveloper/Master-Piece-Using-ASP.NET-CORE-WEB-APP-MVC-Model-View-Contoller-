using Microsoft.AspNetCore.Mvc;

namespace Master_Piece.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Admin_Dashboard()
        {
            return View();
        }
        public IActionResult ManageUsers()
        {
            return View();
        }
        public IActionResult ManageServiceProviders()
        {
            return View();
        }
        public IActionResult ManageServices()
        {
            return View();
        }
    }
}
