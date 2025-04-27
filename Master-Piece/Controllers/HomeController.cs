using System.Diagnostics;
using Master_Piece.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
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
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the same view with validation errors
            }

            // Step 1: Check email and password
            var Loginned_user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.PasswordHash);

            if (Loginned_user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Step 2: Save UserID in session
            HttpContext.Session.SetInt32("UserID", Loginned_user.UserId);


            // Step 3: Retrieve UserRole and Role
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == Loginned_user.UserId);
            if (userRole == null)
            {
                ModelState.AddModelError("", "User role not found.");
                return View(model);
            }

            HttpContext.Session.SetInt32("RoleID", (int)userRole.RoleId);

            var role = _context.Roles.FirstOrDefault(r => r.RoleId == userRole.RoleId);
            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist.");
                return View(model);
            }

            HttpContext.Session.SetString("UserRole", role.RoleName);

            // Step 4: Redirect based on Role
            return role.RoleName switch
            {
                "Guest" => RedirectToAction("Login", "Home"),
                "User" => RedirectToAction("Index", "Home"),
                "Employee" => RedirectToAction("Employee_Dashboard", "Employee"),
                "Manager" => RedirectToAction("Manager_Dashboard", "Manager"),
                "Admin" => RedirectToAction("Index", "Admin"),
                "SuperAdmin" => RedirectToAction("Index", "SuperAdmin"),
                _ => RedirectToAction("Login", "Home")
            };
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register_New_UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.PasswordHash != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(model);
            }

            // Load default image from wwwroot
            string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Guest_User_Image.jpg");
            byte[] defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);

            // Create a new User entity and map from ViewModel
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                PersonalImage = defaultImageBytes
            };

            // Add user to database
            _context.Users.Add(user);
            _context.SaveChanges();

            // Assign "User" role
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == "User");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                });

                _context.SaveChanges();
            }

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
        [HttpPost]
        public async Task<IActionResult> Register_New_Employee(Register_New_EmployeeViewModel model, IFormFile PersonalImage)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map ViewModel to User entity
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                PersonalAddress = model.PersonalAddress,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                WorkerServiceType = model.WorkerServiceType,
            };

            // Handle image upload
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await PersonalImage.CopyToAsync(ms);
                    user.PersonalImage = ms.ToArray();
                }
            }
            else
            {
                // Optionally set a default image here
                string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Guest_User_Image.jpg");
                if (System.IO.File.Exists(defaultImagePath))
                {
                    user.PersonalImage = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                }
            }

            // Save User
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Save Role
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == "Employee");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Login", "Home");
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
