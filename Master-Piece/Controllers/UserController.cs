using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Profile()
        {
            var user = new User
            {
                UserId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0"),
                FirstName = HttpContext.Session.GetString("FirstName"),
                LastName = HttpContext.Session.GetString("LastName"),
                Email = HttpContext.Session.GetString("Email") ?? string.Empty,
                PhoneNumber = HttpContext.Session.GetString("PhoneNumber"),
                Role = HttpContext.Session.GetString("Role"),
                CreatedAt = DateTime.TryParse(HttpContext.Session.GetString("CreatedAt"), out DateTime createdAt) ? (DateTime?)createdAt : null,
                Image = HttpContext.Session.GetString("Image") ?? "~/images/default-user.png",
                Address = HttpContext.Session.GetString("Address"),
                DateOfBirth = DateOnly.TryParse(HttpContext.Session.GetString("DateOfBirth"), out var dob) ? dob : null,
                Gender = HttpContext.Session.GetString("Gender"),
                IsActive = HttpContext.Session.GetString("IsActive") == "true"
            };
            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            var user = new User
            {
                UserId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0"),
                FirstName = HttpContext.Session.GetString("FirstName"),
                LastName = HttpContext.Session.GetString("LastName"),
                Email = HttpContext.Session.GetString("Email") ?? string.Empty,
                PhoneNumber = HttpContext.Session.GetString("PhoneNumber"),
                Role = HttpContext.Session.GetString("Role"),
                CreatedAt = DateTime.TryParse(HttpContext.Session.GetString("CreatedAt"), out DateTime createdAt) ? (DateTime?)createdAt : null,
                Image = HttpContext.Session.GetString("Image") ?? "~/images/default-user.png",
                Address = HttpContext.Session.GetString("Address"),
                DateOfBirth = DateOnly.TryParse(HttpContext.Session.GetString("DateOfBirth"), out var dob) ? dob : null,
                Gender = HttpContext.Session.GetString("Gender"),
                IsActive = HttpContext.Session.GetString("IsActive") == "true"
            };
            return View(user); // Pass the User object to the view
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser)
        {
            
                //var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
                if (updatedUser == null)
                {
                    return NotFound(); // Handle missing user
                }

            // Update user details
            updatedUser.UserId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
                //user.FirstName = updatedUser.FirstName;
                //user.LastName = updatedUser.LastName;
                //user.Email = updatedUser.Email;
                //user.PhoneNumber = updatedUser.PhoneNumber;
                //user.Address = updatedUser.Address;
                //user.DateOfBirth = updatedUser.DateOfBirth;
                //user.Gender = updatedUser.Gender;
                //user.IsActive = updatedUser.IsActive;
                _context.Users.Update(updatedUser);
                _context.SaveChanges(); // Save updates to the database

                // Update session data
                HttpContext.Session.SetString("FirstName", updatedUser.FirstName);
                HttpContext.Session.SetString("LastName", updatedUser.LastName);
                HttpContext.Session.SetString("Email", updatedUser.Email);
                HttpContext.Session.SetString("PhoneNumber", updatedUser.PhoneNumber);
                HttpContext.Session.SetString("Address", updatedUser.Address);
                HttpContext.Session.SetString("DateOfBirth", updatedUser.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty);
                HttpContext.Session.SetString("Gender", updatedUser.Gender);
                HttpContext.Session.SetString("IsActive", updatedUser.IsActive ==true ? "true" : "false");

            return RedirectToAction("Profile");
        }
        public IActionResult BookedServicesHistory()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var bookings = _context.Bookings
                //.Include(b => b.Service)
                //.Where(b => b.UserId == userId)
                //.OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookings);
        }


        public IActionResult Reset_Password()
        {
            return View();
        }
        public IActionResult More_Details_About_ThatWorker(int id)
        {
            // Fetch the worker details from the database or data source
            var worker = _context.ServiceProviders.FirstOrDefault(w => w.ProviderId == id);

            if (worker == null)
            {
                return NotFound(); // Handle case where worker doesn't exist
            }

            return View(worker); // Pass the worker object to the View
        }

        public IActionResult Show_Workers_for_Specfic_Service_(int id)
        //public IActionResult Show_Workers_for_Specfic_Service_()
        {
            //int id = 1;
            // Query to retrieve providers for the specific service
            var providersForService = _context.ServiceWorkersJunctionTables
                .Where(junction => junction.ServiceId == id) // Filter by ServiceId
                .Select(junction => junction.Provider)       // Select related Providers
                .ToList();                                   // Convert to list

            // Pass the retrieved providers to the view
            return View(providersForService);
        }
   
        [HttpGet]
        public IActionResult Show_All_Services()
        {
            var Our_Services=_context.Services.ToList();
            return View(Our_Services);
        }
    }
}
