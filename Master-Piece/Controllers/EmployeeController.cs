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
            //completed tasks
            // Query to retrieve completed tasks
            var completedTasks = (from sp in _context.ServiceProviders
                                  join swjt in _context.ServiceWorkersJunctionTables on sp.ProviderId equals swjt.ProviderId
                                  join s in _context.Services on swjt.ServiceId equals s.ServiceId
                                  join b in _context.Bookings on s.ServiceId equals b.ServiceId
                                  where b.UserId == userId && b.Status == "Completed"
                                  select new
                                  {
                                      ProviderName = sp.WorkerName,
                                      ServiceType = sp.ServiceType,
                                      ServiceName = s.ServiceName,
                                      BookingDate = b.BookingDate,
                                      Status = b.Status
                                  }).ToList();
            // Count the number of completed tasks
            var completedTaskCount = completedTasks.Count;

            // Save the count in ViewBag
            ViewBag.CompletedTaskCount = completedTaskCount;


            //the number task 
            //waiting tasks
            //
            return View();
        }
        public IActionResult Assgined_Task()
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

        public IActionResult WorkHistory()
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
