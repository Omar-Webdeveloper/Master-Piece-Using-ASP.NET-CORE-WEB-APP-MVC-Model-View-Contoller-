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
            int userId = int.Parse(HttpContext.Session.GetString("UserID") ?? "0");

            // Completed tasks
            var completedTasks = (from sp in _context.ServiceProviders
                                  join t in _context.Tasks on sp.ProviderId equals t.ProviderId
                                  join swjt in _context.ServiceWorkersJunctionTables on sp.ProviderId equals swjt.ProviderId
                                  join s in _context.Services on swjt.ServiceId equals s.ServiceId
                                  where sp.ProviderId == userId && t.TaskStatus == "COMPLETED"
                                  select new
                                  {
                                      ProviderName = sp.WorkerName,
                                      ServiceType = sp.ServiceType,
                                      ServiceName = s.ServiceName,
                                      TaskName = t.TaskName,
                                      StartDate = t.StartDate,
                                      EndDate = t.EndDate,
                                      Status = t.TaskStatus
                                  }).ToList();

            // Count the number of completed tasks
            var completedTaskCount = completedTasks.Count;

            // Save the count of completed tasks in ViewBag the 
            ViewBag.CompletedTaskCount = completedTaskCount;

            //customer Rating
            // Get average rating for the provider's booked services
            var averageRating = (from sp in _context.ServiceProviders
                                 join swjt in _context.ServiceWorkersJunctionTables on sp.ProviderId equals swjt.ProviderId
                                 join s in _context.Services on swjt.ServiceId equals s.ServiceId
                                 join b in _context.Bookings on s.ServiceId equals b.ServiceId
                                 join r in _context.Reviews on b.BookingId equals r.BookingId
                                 where sp.ProviderId == userId && b.Status == "Completed"
                                 select r.Rating).ToList();
            var viewbagaverageRating = averageRating.DefaultIfEmpty(0).Average();

            // Save the average rating in ViewBag
            ViewBag.AverageRating = viewbagaverageRating;













            //the employee of the month 
            // Get current year
            int currentYear = DateTime.Now.Year;

            // Query to fetch evaluations for the current year
            var evaluationsForCurrentYear = (from e in _context.Evaluations
                                             where e.ProviderId == userId &&
                                                   e.EvaluationYear == currentYear
                                             select new
                                             {
                                                 e.EvaluationId,
                                                 e.Score,
                                                 e.Comments,
                                                 e.CreatedAt
                                             }).ToList();

            // Example: Output evaluation details
            if (evaluationsForCurrentYear.Any())
            {
                foreach (var evaluation in evaluationsForCurrentYear)
                {
                    Console.WriteLine($"EvaluationID: {evaluation.EvaluationId}, Score: {evaluation.Score}, Comments: {evaluation.Comments}, CreatedAt: {evaluation.CreatedAt}");
                }
            }
            else
            {
                Console.WriteLine("No evaluations found for the current year.");
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
            return View();
        }
        public IActionResult EmployeeEditProfile()
        {
            return View();
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
