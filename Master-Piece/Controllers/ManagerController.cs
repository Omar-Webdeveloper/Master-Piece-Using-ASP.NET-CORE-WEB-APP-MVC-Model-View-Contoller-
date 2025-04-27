using Master_Piece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class ManagerController : Controller
    {
        private readonly MyDbContext _context;

        public ManagerController(MyDbContext context)
        {
            _context = context;
        }
        //        public IActionResult Admin_Dashboard()
        //        {
        //            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

        //            // Make sure you're using your DbContext (e.g., _context)
        //            var totalUsers = _context.Users
        //                .Where(u => u.Role == "User")
        //                .Count();

        //            ViewBag.TotalUsers = totalUsers;

        //             var serviceProviders = _context.Users
        //            .Where(u => u.Role == "ServiceProvider")
        //            .Count();
        //            ViewBag.TotalServiceProviders = serviceProviders;

        //            var totalServices = _context.Services
        //                .Count();
        //            ViewBag.TotalServices = totalServices;

        //            var ManagerName = _context.Users
        //                .Where(u => u.Role == "Manager")
        //                .Select(u => u.FirstName)
        //                .FirstOrDefault();
        //            ViewBag.ManagerName = ManagerName;

        //            return View();
        //        }
        //        public IActionResult ManageUsers()
        //        {
        //            return View();
        //        }
        //        public IActionResult ManageServiceProviders()
        //        {
        //            return View();
        //        }
        //        public IActionResult ManageServices()
        //        {
        //            return View();
        //        }
        //        public IActionResult AssginTask()
        //        {
        //            return View();
        //        }
        public IActionResult ManagerProfile()
        {
            // Retrieve user Id from the session
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);


            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult ManagerEditProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult ManagerEditProfile(User updatedUser, IFormFile PersonalImage)
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Update fields manually
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.PersonalAddress = updatedUser.PersonalAddress;
            existingUser.DateOfBirth = updatedUser.DateOfBirth;
            existingUser.Gender = updatedUser.Gender;
            // (You can update more fields if you want)
            // Handle image upload
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    PersonalImage.CopyToAsync(ms);
                    existingUser.PersonalImage = ms.ToArray();
                }
            }
            else
            {
                // Optionally set a default image here
                string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Guest_User_Image.jpg");
                if (System.IO.File.Exists(defaultImagePath))
                {
                    existingUser.PersonalImage = System.IO.File.ReadAllBytes(defaultImagePath);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("ManagerProfile");
        }
        

        public IActionResult ManagerResetPassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult ManagerResetPassword(string OldPassword, string NewPassword, string ConfirmNewPassword)
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var LoggedInUser = _context.Users.FirstOrDefault(u => u.UserId == userId && u.PasswordHash == OldPassword);
            if (LoggedInUser != null)
            {
                if (NewPassword == ConfirmNewPassword)
                {
                    LoggedInUser.PasswordHash = NewPassword;
                    _context.Users.Update(LoggedInUser);
                    _context.SaveChanges();
                    return RedirectToAction("ManagerProfile");
                }

            }
            else
            {
                // Handle case where old password is incorrect
                ModelState.AddModelError("", "Old password is incorrect.");
                return View();
            }
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
