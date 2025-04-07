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
        public IActionResult About()
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
            if (user == null) {
                return BadRequest("User cannot be null");
            }
            else if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Email and Password cannot be empty");
            }
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash);
            if (existingUser != null) {
                // User found, redirect to the appropriate page based on role
                if (existingUser.Role == "Admin")
                {
                    TempData["Role"] = "Admin";
                    return RedirectToAction("Admin_Dashboard", "Admin");
                }
                else if (existingUser.Role == "Employee")
                {
                    TempData["Role"] = "Employee";
                    return RedirectToAction("Employee_Dashboard", "Employee");
                }
                else if (existingUser.Role == "User")
                {
                    TempData["Role"] = "Employee";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Role"] = "Guest";
            return View(); 
        }
        public IActionResult Register()
        {
            return View();
        }
        //public IActionResult Register(User user, string repeatPassword)
        //{
        //    if (user == null)
        //    {
        //        return BadRequest("User cannot be null");
        //    }
        //    else if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
        //    {
        //        return BadRequest("Email and Password cannot be empty");
        //    }
        //    // Compare PasswordHash and RepeatPassword
        //    if (user.PasswordHash != repeatPassword)
        //    {
        //        ModelState.AddModelError("RepeatPassword", "Passwords do not match.");
        //        return View(); // Return to the view with validation errors
        //    }
        //    // Assign default role and created date
        //    user.Role = "User";
        //    user.CreatedAt = DateTime.Now;

        //    // Add user to database using Entity Framework
        //    _context.Users.Add(user);
        //    _context.SaveChanges();

        //    return View();
        //}
        public IActionResult Testmonials()
        {
            return View();
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
