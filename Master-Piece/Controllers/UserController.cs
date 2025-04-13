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
            // Retrieve user Id from the session
            int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

   
            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Retrieve user Id from the session
            int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);


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
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);

            _context.Users.Update(updatedUser);
                _context.SaveChanges(); // Save updates to the database



            return RedirectToAction("Profile");
        }
        public IActionResult BookedServicesHistory()
        {
            // Retrieve the UserID from the session
            int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");

            // Execute the query to fetch booked services
            //var bookings = from user in _context.Users
            //               join provider in _context.ServiceProviders
            //               on user.UserId equals provider.UserId
            //               join service in _context.Services
            //               on provider.ProviderId equals service.ServiceId
            //               where user.UserId == userId
            //               select new
            //               {
            //                   UserID = user.UserId,
            //                   FullName = user.FirstName + " " + user.LastName,
            //                   WorkerName = provider.WorkerName,
            //                   ServiceType = provider.ServiceType,
            //                   Rating = provider.Rating,
            //                   ServiceName = service.ServiceName,
            //                   Description = service.Description,
            //                   StartingPrice = service.StartingPrices,
            //                   ServiceCreatedAt = service.CreatedAt
            //               };

            // Execute the query and map results to a List<dynamic>
            var bookings = (from user in _context.Users
                            join provider in _context.ServiceProviders
                            on user.UserId equals provider.UserId
                            join service in _context.Services
                            on provider.ProviderId equals service.ServiceId
                            where user.UserId == userId
                            select new
                            {
                                UserID = user.UserId,
                                FullName = user.FirstName + " " + user.LastName,
                                WorkerName = provider.WorkerName,
                                ServiceType = provider.ServiceType,
                                Rating = provider.Rating,
                                ServiceName = service.ServiceName,
                                Description = service.Description,
                                StartingPrice = service.StartingPrices,
                                ServiceCreatedAt = service.CreatedAt
                            })
                            .AsEnumerable() // Convert to IEnumerable for LINQ to Objects
                            .Select(x => (dynamic)x) // Map to dynamic
                            .ToList(); // Convert to List<dynamic>
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
