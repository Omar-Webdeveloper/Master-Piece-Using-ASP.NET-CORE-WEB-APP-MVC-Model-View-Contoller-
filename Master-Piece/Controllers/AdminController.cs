using Microsoft.AspNetCore.Mvc;

namespace Master_Piece.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Admin_Dashboard()
        {
            return View();
        }
        public IActionResult Add_Location()
        {
            return View();
        }
        public IActionResult Add_Manager()
        {
            return View();
        }
        public IActionResult Add_Employee()
        {
            return View();
        }
        public IActionResult Add_User()
        {
            return View();
        }
        public IActionResult Contact_Us_Messsages()
        {
            return View();
        }
        public IActionResult Check_Review_Status()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            HttpContext.Session.Remove("Role");
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Index", "Home");
        }
    }
}
