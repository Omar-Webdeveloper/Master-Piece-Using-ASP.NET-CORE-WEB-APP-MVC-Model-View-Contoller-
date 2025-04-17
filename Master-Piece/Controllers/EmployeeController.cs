using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;

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
            var provider = _context.ServiceProviders.FirstOrDefault(sp => sp.UserId == userId);

            if (provider != null)
            {
                int providerId = provider.ProviderId;

                // Get completed tasks for that provider
                var completedTasks = _context.Tasks
                    .Where(t => t.ProviderId == providerId && t.TaskStatus == "COMPLETED")
                    .Select(t => new
                    {
                        TaskName = t.TaskName,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        Status = t.TaskStatus
                    })
                    .ToList();

                ViewBag.CompletedTaskCount = completedTasks.Count;
                ViewBag.CompletedTasks = completedTasks;











                //customer Rating
                // Get average rating for the provider's booked services
                var averageRating = (from sp in _context.ServiceProviders
                                     join swjt in _context.ServiceWorkersJunctionTables on sp.ProviderId equals swjt.ProviderId
                                     join s in _context.Services on swjt.ServiceId equals s.ServiceId
                                     join b in _context.Bookings on s.ServiceId equals b.ServiceId
                                     join r in _context.Reviews on b.BookingId equals r.BookingId
                                     where sp.ProviderId == providerId && b.Status == "Completed"
                                     select r.Rating).ToList();
                var viewbagaverageRating = averageRating.DefaultIfEmpty(0).Average();

                // Save the average rating in ViewBag
                ViewBag.AverageRating = viewbagaverageRating;




                // First, check if the provider exists and has an evaluation score of 5 and no comments
                var isTopEvaluated = _context.Evaluations
                    .Any(e => e.ProviderId == providerId && e.Score == 5 && (e.Comments == null || e.Comments == ""));


                // Check both conditions
                bool isEmployeeOfMonth = isTopEvaluated && viewbagaverageRating > 3;

                // Pass result to ViewBag
                ViewBag.IsEmployeeOfMonth = isEmployeeOfMonth;


                if (isEmployeeOfMonth)
                {
                    var workerName = _context.ServiceProviders
                        .Where(sp => sp.ProviderId == providerId)
                        .Select(sp => sp.WorkerName)
                        .FirstOrDefault();

                    ViewBag.WorkerName = workerName;
                }







                DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                // التحويل إلى DateOnly للمقارنة
                DateOnly startDateOnly = DateOnly.FromDateTime(startOfMonth);
                DateOnly endDateOnly = DateOnly.FromDateTime(endOfMonth);

                var allCompletedTasks = _context.Tasks
              .Where(t => t.ProviderId == providerId &&
                          t.TaskStatus == "COMPLETED" &&
                          t.EndDate >= startDateOnly &&
                          t.EndDate <= endDateOnly)
              .ToList();

                int totalTasks = allCompletedTasks.Count;

                // تقسيم الشهر إلى 4 أسابيع (تقريبًا 7 أيام لكل أسبوع)
                int daysInMonth = (endOfMonth - startOfMonth).Days + 1;
                int weekLength = daysInMonth / 4;

                var weeklyCumulative = new List<int>();
                int cumulative = 0;

                for (int i = 0; i < 4; i++)
                {
                    var weekStart = startOfMonth.AddDays(i * weekLength);
                    var weekEnd = (i == 3) ? endOfMonth : weekStart.AddDays(weekLength - 1);

                    // تحويل لـ DateOnly للمقارنة
                    DateOnly weekStartDateOnly = DateOnly.FromDateTime(weekStart);
                    DateOnly weekEndDateOnly = DateOnly.FromDateTime(weekEnd);

                    int weekCount = allCompletedTasks
                        .Count(t => t.EndDate.HasValue &&
                                    t.EndDate.Value >= weekStartDateOnly &&
                                    t.EndDate.Value <= weekEndDateOnly);

                    cumulative += weekCount;
                    weeklyCumulative.Add(cumulative);
                }


                ViewBag.WeeklyLabels = new List<string> { "1/4", "2/4", "3/4", "4/4" };
                ViewBag.WeeklyCumulativeCounts = weeklyCumulative;
                ViewBag.MaxCount = totalTasks;








            }

            var joinedDate = (from sp in _context.ServiceProviders
                              where sp.UserId == userId
                              select sp.RegisterAt).FirstOrDefault();

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
































































            //waiting tasks
            //
            return View();
        }
        public IActionResult Assgined_Task()
        {
            return View();
        }
        public IActionResult WorkHistory()
        {
            return View();
        }
        public IActionResult EmployeeProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var profile = _context.ServiceProviders
                .Include(sp => sp.User)
                .FirstOrDefault(sp => sp.UserId == userId);
            return View(profile);
        }
        public IActionResult EmployeeEditProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var profile = _context.ServiceProviders
                .Include(sp => sp.User)
                .FirstOrDefault(sp => sp.UserId == userId);
            return View(profile);
        }
        public IActionResult EmployeeResetPassword()
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
    }
}
