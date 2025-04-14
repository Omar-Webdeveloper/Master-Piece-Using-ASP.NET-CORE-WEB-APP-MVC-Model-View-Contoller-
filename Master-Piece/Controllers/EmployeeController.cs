using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MyDbContext _context;

        public EmployeeController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Employee_Dashboard()
        {
            return View();
        }
        public IActionResult Assgined_Task()
        {
            return View();
        }
        public IActionResult EmployeeProfile()
        {
            return View();
        }
        public IActionResult EmployeeEditProfile()
        {
            return View();
        }
        public IActionResult EmployeeResetPassword()
        {
            return View();
        }

        public IActionResult WorkHistory()
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
