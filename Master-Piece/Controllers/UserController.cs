using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.IdentityModel.Tokens;

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

            // Execute the query and map results to a List<dynamic>
            var bookings = (from booking in _context.Bookings
                            join service in _context.Services
                            on booking.ServiceId equals service.ServiceId
                            join junction in _context.ServiceWorkersJunctionTables
                            on service.ServiceId equals junction.ServiceId
                            join provider in _context.ServiceProviders
                            on junction.ProviderId equals provider.ProviderId
                            where booking.UserId == userId
                            select new
                            {
                                BookingID = booking.BookingId,
                                WorkerName = provider.WorkerName,
                                ServiceType = provider.ServiceType,
                                ServiceName = service.ServiceName,
                                ServiceDescription = service.Description,
                                StartingPrice = service.StartingPrices,
                                ServiceCreatedAt = service.CreatedAt,
                                BookingDate = booking.BookingDate,
                                BookingStatus = booking.Status,
                                BookingCreatedAt = booking.CreatedAt
                            })
                            .AsEnumerable() // Convert to IEnumerable for LINQ to Objects
                            .Select(x => (dynamic)x) // Map to dynamic
                            .ToList(); // Convert to List<dynamic>
            
                Console.WriteLine($"BookingID: {bookings}");
     
            return View(bookings);
        }
        [HttpGet]
        public IActionResult Reset_Password()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset_Password(string OldPassword, string NewPassword, string ConfirmNewPassword)
        {
            // 1. Get current user ID (assuming it's stored in session)
            int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID") ?? "0"))
            {
                TempData["Error"] = "You must be logged in to change your password.";
                return RedirectToAction("Login", "Home");
            }

            //int userId = int.Parse(userIdString);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Home");
            }

            // 2. Check old password
            if (user.PasswordHash != OldPassword)
            {
                TempData["Error"] = "Old password is incorrect.";
                return View();
            }

            // 3. Validate new password match
            if (NewPassword != ConfirmNewPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return View();
            }

            // 4. Save new password
            user.PasswordHash = NewPassword;
            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("Profile"); // Or wherever you want to redirect
        }


        //public IActionResult Reset_Password()
        //{

        //    return View();
        //}
        //[HttpPut]
        //public IActionResult Reset_Password(string OldPassword, string NewPassword,string ConfirmNewpassword)
        //{
        //    int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
        //    var LoggedInUser = _context.Users.FirstOrDefault(u => u.UserId == userId && u.PasswordHash == OldPassword);
        //    if (LoggedInUser != null)
        //    {
        //        if(NewPassword == ConfirmNewpassword) 
        //        {
        //            LoggedInUser.PasswordHash = NewPassword;
        //            _context.Users.Update(LoggedInUser);
        //            _context.SaveChanges();
        //            return RedirectToAction("Profile");
        //        }

        //    }
        //    else
        //    {
        //        // Handle case where old password is incorrect
        //        ModelState.AddModelError("", "Old password is incorrect.");
        //        return View();
        //    }
        //    return View();
        //}
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
