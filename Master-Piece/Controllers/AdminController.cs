using Master_Piece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Admin_Dashboard()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Make sure you're using your DbContext (e.g., _context)
            var totalUsers = _context.Users
                .Where(u => u.Role == "User")
                .Count();

            ViewBag.TotalUsers = totalUsers;

             var serviceProviders = _context.Users
            .Where(u => u.Role == "ServiceProvider")
            .Count();
            ViewBag.TotalServiceProviders = serviceProviders;

            var totalServices = _context.Services
                .Count();
            ViewBag.TotalServices = totalServices;

            var ManagerName = _context.Users
                .Where(u => u.Role == "Manager")
                .Select(u => u.FirstName)
                .FirstOrDefault();
            ViewBag.ManagerName = ManagerName;

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
        public IActionResult AssginTask()
        {
            return View();
        }
        public IActionResult ManagerProfile()
        {
            return View();
        }
        public IActionResult ManagerEditProfile()
        {
            return View();
        }
        public IActionResult ManagerResetPassword()
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
