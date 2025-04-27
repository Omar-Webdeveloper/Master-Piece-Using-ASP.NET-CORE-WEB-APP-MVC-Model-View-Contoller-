using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Master_Piece.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MyDbContext _context;

        public EmployeeController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Employee_Dashboard()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Find the provider ID using the user ID
            var Worker = _context.Users.FirstOrDefault(sp => sp.UserId == userId);

            if (Worker != null)
            {
                int providerId = Worker.UserId;

                // Count only the completed bookings related to the logged-in worker
                int completedBookingsCount = _context.Bookings
                    .Where(b => b.UserId == providerId && b.Status == "Completed")
                    .Count();

                ViewBag.CompletedBookings = completedBookingsCount;









                // Get the IDs of completed bookings for the logged-in provider
                var completedBookingIdsList = _context.Bookings
                    .Where(b => b.UserId == providerId && b.Status == "Completed")
                    .Select(b => b.BookingId)
                    .ToList(); // <-- ToList() so we can use .Contains later

                // Get the reviews for these completed bookings
                var reviewsQuery = _context.Reviews
                    .Where(r => completedBookingIdsList.Contains((int)r.BookingId) && r.ReviewStatus == "Accepted_To_Show_All");

                // Now count and average
                int completedBookingsCountNow = completedBookingIdsList.Count;
                double averageRating = (double)(reviewsQuery.Any() ? reviewsQuery.Average(r => r.Rating) : 0);

                // Save to ViewBag
                ViewBag.CompletedBookingsCount = completedBookingsCountNow;
                ViewBag.c = averageRating;






                //                // First, check if the provider exists and has an evaluation score of 5 and no comments
                //                var isTopEvaluated = _context.Evaluations
                //                    .Any(e => e.ProviderId == providerId && e.Score == 5 && (e.Comments == null || e.Comments == ""));


                //                // Check both conditions
                //                bool isEmployeeOfMonth = isTopEvaluated && viewbagaverageRating > 3;

                //                // Pass result to ViewBag
                //                ViewBag.IsEmployeeOfMonth = isEmployeeOfMonth;


                //                if (isEmployeeOfMonth)
                //                {
                //                    var workerName = _context.ServiceProviders
                //                        .Where(sp => sp.ProviderId == providerId)
                //                        .Select(sp => sp.WorkerName)
                //                        .FirstOrDefault();

                //                    ViewBag.WorkerName = workerName;
                //                }







                //                DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                //                // التحويل إلى DateOnly للمقارنة
                //                DateOnly startDateOnly = DateOnly.FromDateTime(startOfMonth);
                //                DateOnly endDateOnly = DateOnly.FromDateTime(endOfMonth);

                //                var allCompletedTasks = _context.Tasks
                //              .Where(t => t.ProviderId == providerId &&
                //                          t.TaskStatus == "COMPLETED" &&
                //                          t.EndDate >= startDateOnly &&
                //                          t.EndDate <= endDateOnly)
                //              .ToList();

                //                int totalTasks = allCompletedTasks.Count;

                //                // تقسيم الشهر إلى 4 أسابيع (تقريبًا 7 أيام لكل أسبوع)
                //                int daysInMonth = (endOfMonth - startOfMonth).Days + 1;
                //                int weekLength = daysInMonth / 4;

                //                var weeklyCumulative = new List<int>();
                //                int cumulative = 0;

                //                for (int i = 0; i < 4; i++)
                //                {
                //                    var weekStart = startOfMonth.AddDays(i * weekLength);
                //                    var weekEnd = (i == 3) ? endOfMonth : weekStart.AddDays(weekLength - 1);

                //                    // تحويل لـ DateOnly للمقارنة
                //                    DateOnly weekStartDateOnly = DateOnly.FromDateTime(weekStart);
                //                    DateOnly weekEndDateOnly = DateOnly.FromDateTime(weekEnd);

                //                    int weekCount = allCompletedTasks
                //                        .Count(t => t.EndDate.HasValue &&
                //                                    t.EndDate.Value >= weekStartDateOnly &&
                //                                    t.EndDate.Value <= weekEndDateOnly);

                //                    cumulative += weekCount;
                //                    weeklyCumulative.Add(cumulative);
                //                }


                //                ViewBag.WeeklyLabels = new List<string> { "1/4", "2/4", "3/4", "4/4" };
                //                ViewBag.WeeklyCumulativeCounts = weeklyCumulative;
                //                ViewBag.MaxCount = totalTasks;








            }

            //            var joinedDate = (from sp in _context.ServiceProviders
            //                              where sp.UserId == userId
            //                              select sp.RegisterAt).FirstOrDefault();

            //            if (joinedDate != null)
            //            {
            //                int yearsWithUs = DateTime.Now.Year - joinedDate.Value.Year;

            //                // Adjust if their anniversary hasn't occurred yet this year
            //                if (DateTime.Now.Date < joinedDate.Value.AddYears(yearsWithUs))
            //                {
            //                    yearsWithUs--;
            //                }

            //                ViewBag.YearsWithUs = yearsWithUs;
            //            }
            //            else
            //            {
            //                ViewBag.YearsWithUs = 0;
            //            }
































































            //waiting tasks
            //
            return View();
        }
        //        public IActionResult Assgined_Task()
        //        {
        //            return View();
        //        }
        //        public IActionResult WorkHistory()
        //        {
        //            return View();
        //        }
        public IActionResult EmployeeProfile()
        {
            // Retrieve user Id from the session
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);


            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult EmployeeEditProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult EmployeeEditProfile(User updatedUser, IFormFile PersonalImage)
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

            return RedirectToAction("EmployeeProfile");
        }
        public IActionResult EmployeeResetPassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult EmployeeResetPassword(string OldPassword, string NewPassword, string ConfirmNewPassword)
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            HttpContext.Session.Remove("Role");
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Index", "Home");
        }
    }
    }
