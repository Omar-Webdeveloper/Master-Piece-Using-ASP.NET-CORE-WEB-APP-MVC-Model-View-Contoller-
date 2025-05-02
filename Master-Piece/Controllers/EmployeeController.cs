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
            int workerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            if (workerId != null)
            {

                // Count only the completed bookings related to the logged-in worker
                int completedBookingsCount = _context.Bookings
                    .Where(b => b.WorkerId == workerId && b.Status == "Completed")
                    .Count();

                ViewBag.CompletedBookings = completedBookingsCount;









                // Get the IDs of completed bookings for the logged-in provider
                var completedBookingIdsList = _context.Bookings
                    .Where(b => b.WorkerId == workerId && b.Status == "Completed")
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





                var joinedDate = (from sp in _context.Users
                                  where sp.UserId == workerId
                                  select sp.CreatedAt).FirstOrDefault();

                if (joinedDate != null)
                {
                    int yearsWithUs = DateTime.Now.Year - joinedDate.Value.Year;

                    // Adjust if their anniversary hasn't occurred yet this year
                    if (DateTime.Now.Date < joinedDate.Value.AddYears(yearsWithUs))
                    {
                        yearsWithUs--;
                    }

                    ViewBag.YearsWithUs = yearsWithUs;
                }
                else
                {
                    ViewBag.YearsWithUs = 0;
                }




                // First, check if the provider exists and has an evaluation score of 5 and no comments
                var isTopEvaluated = _context.Evaluations
                    .Any(e => e.WrokerId == workerId && e.Score == 5 && (e.Comments == null || e.Comments == ""));


                // Check both conditions
                bool isEmployeeOfMonth = isTopEvaluated && averageRating > 3;

                // Pass result to ViewBag
                ViewBag.IsEmployeeOfMonth = isEmployeeOfMonth;


                if (isEmployeeOfMonth)
                {
                    var workerName = (from sp in _context.Users
                                      where sp.UserId == workerId
                                      select new { sp.FirstName, sp.LastName }).FirstOrDefault();

                    string fullName = workerName != null ? $"{workerName.FirstName} {workerName.LastName}" : "Orange Coding Academy";


                    ViewBag.WorkerName = fullName;
                }




                // Get the current month and year
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // Get completed jobs grouped by BookingEndDate
                var completedJobsByDay = _context.Bookings
                    .Where(b => b.WorkerId == workerId
                             && b.Status == "Completed"
                             && b.BookingEndDate >= firstDayOfMonth
                             && b.BookingEndDate <= lastDayOfMonth)
                    .GroupBy(b => b.BookingEndDate.Value.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Fill days with 0 if no job completed that day
                var chartLabels = new List<string>();
                var chartData = new List<int>();

                for (var date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    chartLabels.Add(date.ToString("MMM d")); // e.g., Apr 2
                    var dayRecord = completedJobsByDay.FirstOrDefault(d => d.Date == date);
                    chartData.Add(dayRecord?.Count ?? 0);
                }

                // Pass to view
                ViewBag.ChartLabels = chartLabels;
                ViewBag.ChartData = chartData;
                return View();

            }

            else {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult Assgined_Task()
        {
            int workerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // 2. Query bookings assigned to this employee
            var assignedBookings = (from b in _context.Bookings
                                    join u in _context.Users on b.UserId equals u.UserId
                                    join s in _context.MainServices on b.ServiceId equals s.ServiceId
                                    where  b.WorkerId == workerId
                                    select new AssignedBookingViewModel
                                    {
                                        FirstName = u.FirstName,
                                        Email = u.Email,
                                        Personal_Address = u.PersonalAddress,
                                        PhoneNumber = u.PhoneNumber,
                                        ServiceName = s.ServiceName,
                                        BookingId = b.BookingId,
                                        Booking_Start_Date = (DateTime)b.BookingStartDate,
                                        BookingTittle = b.BookingTittle,
                                        BookingMessae = b.BookingMessae,
                                        BookingNotes = b.BookingNotes,
                                        ImageWhereTheIssueLocated = b.ImageWhereTheIssueLocated,
                                        ImageAfterFixing = b.ImageAfterFixing,
                                        Status = b.Status,
                                        WorkerID = b.WorkerId
                                    }).ToList();
            return View(assignedBookings);
        }
        public IActionResult ConfirmBooking(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                booking.Status = "Confirmed";
                _context.SaveChanges();
            }

            return RedirectToAction("Assgined_Task");
        }
        [HttpPost]
        public async Task<IActionResult> CompleteBooking(int BookingId, IFormFile FixedImage)
        {
            var booking = await _context.Bookings.FindAsync(BookingId);

            if (booking != null)
            {
                if (FixedImage != null && FixedImage.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await FixedImage.CopyToAsync(ms);
                        booking.ImageAfterFixing = ms.ToArray(); // Ensure your model has this property
                    }
                }

                booking.Status = "Completed"; // Mark as completed
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Assgined_Task");
        }

        public IActionResult WorkHistory()
        {
            int workerId = HttpContext.Session.GetInt32("UserID") ?? 0;
            
            var Work_history = (from b in _context.Bookings
                                join u in _context.Users on b.UserId equals u.UserId
                                join s in _context.MainServices on b.ServiceId equals s.ServiceId
                                where b.WorkerId == workerId && b.Status == "Completed"
                                select new AssignedBookingViewModel
                                {
                                    FirstName = u.FirstName,
                                    ServiceName = s.ServiceName,
                                    Booking_Start_Date = (DateTime)b.BookingStartDate,
                                    BookingTittle = b.BookingTittle,
                                    BookingMessae = b.BookingMessae,
                                    BookingNotes = b.BookingNotes,
                                    ImageWhereTheIssueLocated = b.ImageWhereTheIssueLocated,
                                    ImageAfterFixing = b.ImageAfterFixing,
                                    Status = b.Status,
                                }).ToList();
            return View(Work_history);
        }
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
            return View(user); // View shows current info
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeEditProfile(User updatedUser, IFormFile PersonalImage)
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Optional validation check
            //if (!ModelState.IsValid)
            //{
            //    return View(updatedUser);
            //}

            // Update user info
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.PersonalAddress = updatedUser.PersonalAddress;
            existingUser.DateOfBirth = updatedUser.DateOfBirth;
            existingUser.Gender = updatedUser.Gender;

            // Handle image upload
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await PersonalImage.CopyToAsync(ms);
                    existingUser.PersonalImage = ms.ToArray();
                }
            }

            await _context.SaveChangesAsync(); // Save the updated user

            return RedirectToAction("EmployeeProfile"); // Redirect to profile view
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
                    return RedirectToAction("EmployeeProfile");
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
