using System.Diagnostics;
using Master_Piece.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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

        public IActionResult About_US()
        {
            return View();
        }
        public IActionResult Contact_US()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitContact(ContactU contact)
        {
            if (ModelState.IsValid)
            {
                _context.ContactUs.Add(contact);
                _context.SaveChanges();
                return RedirectToAction("ThankYou"); // Redirect to a "Thank You" page after submission
            }

            return View("Contact"); // Return to the form if validation fails
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Session.SetString("Role", "Guest");
                return View();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash);
            //if (existingUser != null) {
            //    // User found, redirect to the appropriate page based on role
            //    if (existingUser.Role == "Manager")
            //    {
            //        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
            //        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);

            //        return RedirectToAction("Admin_Dashboard", "Admin");
            //    }
            //    else if (existingUser.Role == "ServiceProvider")
            //    {
            //        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
            //        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);

            //        return RedirectToAction("Employee_Dashboard", "Employee");
            //    }
            //    else if (existingUser.Role == "User")
            //    {
            //        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
            //        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            try
            {
                if (existingUser != null)
                {
                    // User found, redirect to the appropriate page based on role
                    if (existingUser.Role == "Manager")
                    {
                        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
                        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);

                        return RedirectToAction("Admin_Dashboard", "Admin");
                    }
                    else if (existingUser.Role == "ServiceProvider")
                    {
                        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
                        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);

                        return RedirectToAction("Employee_Dashboard", "Employee");
                    }
                    else if (existingUser.Role == "User")
                    {
                        HttpContext.Session.SetString("UserID", existingUser.UserId.ToString());
                        HttpContext.Session.SetString("Role", existingUser.Role ?? string.Empty);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception to the console
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Optionally, log the stack trace for debugging
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Handle the error gracefully
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View("Error"); // Redirect to an error view
            }
            // If user not found or credentials are invalid
            HttpContext.Session.SetString("Role", "Guest");
            TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string RepeatPassword)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Session.SetString("Role", "Guest");
                return View(user);
            }
            if (user.PasswordHash != RepeatPassword)
            {
                return View();
            }
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
            HttpContext.Session.Clear(); // Clears all session data
            HttpContext.Session.Remove("Role");
            Response.Cookies.Delete(".AspNetCore.Session");
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
