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
            var userProfile = new
            {
                UserID = HttpContext.Session.GetString("UserID"), // Retrieve UserID
                FullName = $"{HttpContext.Session.GetString("FirstName")} {HttpContext.Session.GetString("LastName")}", // Combine first and last name
                FirstName = HttpContext.Session.GetString("FirstName"),
                LastName = HttpContext.Session.GetString("LastName"),
                Email = HttpContext.Session.GetString("Email"),
                PhoneNumber = HttpContext.Session.GetString("PhoneNumber"),
                Role = HttpContext.Session.GetString("Role"),
                CreatedAt = HttpContext.Session.GetString("CreatedAt"),
                Image = HttpContext.Session.GetString("Image") ?? "~/images/default-user.png", // Fallback for profile image
                Address = HttpContext.Session.GetString("Address"),
                DateOfBirth = HttpContext.Session.GetString("DateOfBirth"),
                Gender = HttpContext.Session.GetString("Gender"),
                IsActive = HttpContext.Session.GetString("IsActive") // Retrieve Active Status
            };
            return View(userProfile);
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        public IActionResult BookedServicesHistory()
        {
            return View();
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
