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
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);


            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser, IFormFile PersonalImage)
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

            return RedirectToAction("Profile");
        }

        public IActionResult BookedServicesHistory()
        {
            // Retrieve the UserID from the session
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;


            if (userId == 0)
            {
                return RedirectToAction("Login", "Home");
            }

            // Check if logged in user is of role "User"
            var roleName = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.RoleId
                            where ur.UserId == userId
                            select r.RoleName).FirstOrDefault();

            if (roleName != "User")
            {
                return Unauthorized(); // or redirect somewhere else
            }

            var bookingHistory = (from booking in _context.Bookings
                                  join service in _context.MainServices on booking.ServiceId equals service.ServiceId
                                  join sw in _context.ServiceWorkersJunctionTables on booking.ServiceId equals sw.ServiceId
                                  join employee in _context.Users on sw.WrokerId equals employee.UserId
                                  where booking.UserId == userId
                                  select new BookingHistoryViewModel
                                  {
                                      BookingId = booking.BookingId,
                                      BookingTittle = booking.BookingTittle,
                                      BookingMessae = booking.BookingMessae,
                                      Status = booking.Status,
                                      BookingStartDate = booking.BookingStartDate,
                                      ServiceName = service.ServiceName,
                                      ServicePrice = service.ServicePrice,
                                      EmployeeFullName = employee.FirstName + " " + employee.LastName,
                                      IssueImage = booking.ImageWhereTheIssueLocated
                                  }).Distinct().ToList();


            return View(bookingHistory);
        }

        [HttpPost]
        public IActionResult CreatePayment(Payment model)
        {
            _context.Payments.Add(model);
            _context.SaveChanges();
            return RedirectToAction("BookedServicesHistory", "User");
        }

        [HttpPost]
        public IActionResult CreateReview(Review model)
        {
            model.CreatedAt = DateTime.Now;
            model.ReviewStatus = "Pending";
            _context.Reviews.Add(model);
            _context.SaveChanges();
            return RedirectToAction("BookedServicesHistory", "User");
        }


        public IActionResult Reset_Password()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Reset_Password(string OldPassword, string NewPassword, string ConfirmNewPassword)
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
                    return RedirectToAction("Profile");
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



        [HttpGet]
        public IActionResult Show_All_Services()
        {
            var Our_Services = _context.MainServices.ToList();
            return View(Our_Services);
        }


        public IActionResult Show_Workers_for_Specfic_Service_(int id)
        {
            HttpContext.Session.SetInt32("ServiceId", id);
            int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            if (serviceId == null)
            {
                // handle null service id (maybe redirect or return error)
                return RedirectToAction("SelectService", "Home");
            }

            var Employees_For_Serivce = (from u in _context.Users
                             join ur in _context.UserRoles on u.UserId equals ur.UserId
                             join r in _context.Roles on ur.RoleId equals r.RoleId
                             join sw in _context.ServiceWorkersJunctionTables on u.UserId equals sw.WrokerId
                             where r.RoleName == "Employee" && sw.ServiceId == serviceId
                             select u).ToList();

            var viewModel = new ShowWorkersAndBookingViewModel
            {
                Employees = Employees_For_Serivce,
                ServiceId = serviceId.Value // Make sure it's not null

            };

            return View(viewModel);
        }









        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SubmitBooking(ShowWorkersAndBookingViewModel model, IFormFile bookingImage, int ServiceId)
        {
            // Check if user is logged in
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Get values from session
            int? userId = HttpContext.Session.GetInt32("UserID");
            //int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            // Set session values to booking
            model.NewBooking.UserId = userId;
            model.NewBooking.ServiceId = model.ServiceId;
            model.NewBooking.BookingStartDate = DateTime.Now;
            model.NewBooking.Status = "Pending";

            // Handle image upload if provided
            if (bookingImage != null && bookingImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await bookingImage.CopyToAsync(memoryStream);
                    model.NewBooking.ImageWhereTheIssueLocated = memoryStream.ToArray();
                }
            }

            // Save to database
            _context.Bookings.Add(model.NewBooking);
            await _context.SaveChangesAsync();

            // Redirect or show confirmation
            return RedirectToAction("BookedServicesHistory", "User"); // or any success page
        }






        public IActionResult More_Details_About_ThatWorker(int id, int serviceId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null )
            {
                return RedirectToAction("Login", "Account"); // or wherever your login is
            }

            var workerDetails = (from u in _context.Users
                                 join ur in _context.UserRoles on u.UserId equals ur.UserId
                                 join r in _context.Roles on ur.RoleId equals r.RoleId
                                 join sw in _context.ServiceWorkersJunctionTables on u.UserId equals sw.WrokerId
                                 where r.RoleName == "Employee"
                                     && sw.ServiceId == serviceId
                                     && u.UserId == id
                                 select u).FirstOrDefault();


            if (workerDetails == null)
            {
                return NotFound();
            }

            var viewModel = new WorkerDetailsAndPreBookingViewModel
            {
                Employee = workerDetails,
                NewBooking = new Booking
                {
                    UserId = userId.Value,
                    ServiceId = serviceId,
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPreBooking(WorkerDetailsAndPreBookingViewModel model, IFormFile bookingImage, int ServiceId)
        {
            // Check if user is logged in
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Get values from session
            int? userId = HttpContext.Session.GetInt32("UserID");

            // Set session values to booking
            model.NewBooking.ServiceId = model.ServiceId;
            model.NewBooking.UserId = userId;
            model.NewBooking.Status = "Pending";

            // Handle image upload if provided
            if (bookingImage != null && bookingImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await bookingImage.CopyToAsync(memoryStream);
                    model.NewBooking.ImageWhereTheIssueLocated = memoryStream.ToArray();
                }
            }

            // Save to database
            _context.Bookings.Add(model.NewBooking);
            await _context.SaveChangesAsync();

            // Redirect or show confirmation
            return RedirectToAction("BookedServicesHistory", "User"); // or any success page
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitPayment(int bookingId, decimal amountPaid, string paymentNotes)
        {
            // Save payment to database linked to bookingId
            // Example pseudo-code
            /*
            var payment = new Payment
            {
                BookingId = bookingId,
                AmountPaid = amountPaid,
                PaymentNotes = paymentNotes,
                PaymentDate = DateTime.Now
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();
            */

            return RedirectToAction("BookingHistory"); // or wherever you want
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitReview(int bookingId, int rating, string reviewText)
        {
            // Save review to database linked to bookingId
            // Example pseudo-code
            /*
            var review = new Review
            {
                BookingId = bookingId,
                Rating = rating,
                ReviewText = reviewText,
                ReviewDate = DateTime.Now
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            */

            return RedirectToAction("BookingHistory");
        }

    }
}
