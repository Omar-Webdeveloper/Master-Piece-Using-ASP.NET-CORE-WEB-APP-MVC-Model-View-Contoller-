using System.Diagnostics;
using Master_Piece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult About_US()
        {
            return View();
        }
        public IActionResult Contact_US()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        { 

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash);
            if (existingUser != null) {
                // User found, redirect to the appropriate page based on role
                if (existingUser.Role == "Manager")
                {
                    TempData["Role"] = "Admin";
                    return RedirectToAction("Admin_Dashboard", "Admin");
                }
                else if (existingUser.Role == "ServiceProvider")
                {
                    TempData["Role"] = "Employee";
                    return RedirectToAction("Employee_Dashboard", "Employee");
                }
                else if (existingUser.Role == "User")
                {
                    TempData["Role"] = "User";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Role"] = "Guest";
            // If user not found or credentials are invalid
            TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
            return View(); 
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user, string repeatPassword)
        {
 
            // Assign default role and created date
            user.Role = "User"; 
            user.Image = "Waiting";
            user.CreatedAt = DateTime.Now;

            // Add user to database using Entity Framework
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login", "Home");

        }
        public IActionResult Forget_Password()
        {
            return View();
        }
        public IActionResult Register_New_Employee()
        {
            return View();
        }

        public IActionResult Testmonials()
        {
            return View();
        }
        public IActionResult Logout() 
        { 
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
