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
        public IActionResult Manager_Dashboard()
        {
            int ManagerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Step 1: Get the Location_Id for the current manager
            var managerLocationId = _context.Users
                .Where(u => u.UserId == ManagerId)
                .Select(u => u.LocationId)
                .FirstOrDefault();

            // Step 2: Get RoleID for 'Employee'
            var employeeRoleId = _context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstOrDefault();

            // Step 3: Get all users with that role and matching location
            var employees = (from u in _context.Users
                             join ur in _context.UserRoles on u.UserId equals ur.UserId
                             where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId
                             select u).Count();

            // Store in ViewBag
            ViewBag.EmployeeCount = employees;







            // Get the raw comma-separated area strings for this manager
            var rawAreas = _context.LocationAreas
                .Where(l => l.ManagerId == ManagerId)
                .Select(l => l.AreasCovered)
                .ToList();  // List of strings

            // Split by comma and trim whitespace, then flatten into a single list
            var coveredAreas = rawAreas
                .SelectMany(a => a.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(a => a.Trim())
                .ToArray();  // Final cleaned array of individual area names

            // Store in ViewBag
            ViewBag.CoveredAreas = coveredAreas;
            ViewBag.CoveredAreaCount = coveredAreas.Length;








            var manager = _context.Users
                .Where(u => u.UserId == ManagerId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefault();

            if (manager != null)
            {
                ViewBag.ManagerFullName = $"{manager.FirstName} {manager.LastName}";
            }
            else
            {
                ViewBag.ManagerFullName = "Unknown Manager";
            }




            int managerId = HttpContext.Session.GetInt32("UserID") ?? 0;





            // Step 3: Get all employee IDs under the same location
            var employeeIds = (from u in _context.Users
                               join ur in _context.UserRoles on u.UserId equals ur.UserId
                               where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId
                               select u.UserId).ToList();

            // Step 4: Get completed bookings by those employees, grouped by date
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var completedJobsByDate = _context.Bookings
                .Where(b => employeeIds.Contains(b.WorkerId ?? 0)
                            && b.Status == "Completed"
                            && b.BookingEndDate.HasValue
                            && b.BookingEndDate.Value.Month == currentMonth
                            && b.BookingEndDate.Value.Year == currentYear)
                .GroupBy(b => b.BookingEndDate.Value.Date)
                .Select(g => new
                {
                    Day = g.Key.Day,
                    Count = g.Count()
                })
                .OrderBy(x => x.Day)
                .ToList();

            // Step 5: Prepare arrays for chart
            ViewBag.ChartLabels = completedJobsByDate.Select(x => x.Day.ToString()).ToArray();
            ViewBag.ChartData = completedJobsByDate.Select(x => x.Count).ToArray();


            return View();
        }
        public IActionResult Manage_New_ServiceProviders()
        {
            int ManagerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Step 1: Get the Location_Id for the current manager
            var managerLocationId = _context.Users
                .Where(u => u.UserId == ManagerId)
                .Select(u => u.LocationId)
                .FirstOrDefault();

            // Step 2: Get RoleID for 'Employee'
            var employeeRoleId = _context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstOrDefault();

            var employeesWithServices = (from u in _context.Users
                                         join ur in _context.UserRoles on u.UserId equals ur.UserId
                                         join sw in _context.ServiceWorkersJunctionTables on u.UserId equals sw.WrokerId into swj
                                         from sw in swj.DefaultIfEmpty()
                                         join s in _context.MainServices on sw.ServiceId equals s.ServiceId into sj
                                         from s in sj.DefaultIfEmpty()
                                         where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId && sw.Status == "Pending"
                                         select new EmployeeWithServiceViewModel
                                         {
                                             UserId = u.UserId,
                                             FirstName = u.FirstName,
                                             LastName = u.LastName,
                                             Email = u.Email,
                                             CreatedAt = u.CreatedAt,
                                             PersonalImage = u.PersonalImage,
                                             PersonalAddress = u.PersonalAddress,
                                             DateOfBirth = u.DateOfBirth,
                                             PhoneNumber = u.PhoneNumber,
                                             Gender = u.Gender,
                                             LocationId = u.LocationId,
                                             WorkerServiceType = u.WorkerServiceType,
                                             WorkerRating = u.WorkerRating,
                                             WorkerIntro = u.WorkerIntro,
                                             IsActive = u.IsActive,
                                             ServiceName = s.ServiceName,
                                             Status = sw.Status,
                                             ServiceId = (int)sw.ServiceId
                                         }).ToList();

            return View(employeesWithServices);
        }
        [HttpPost]
        public IActionResult ApproveServiceProvider(int userId, int serviceId)
        {
            // Find the ServiceWorkersJunctionTable entry
            var serviceWorker = _context.ServiceWorkersJunctionTables
                .FirstOrDefault(sw => sw.WrokerId == userId && sw.ServiceId == serviceId);
            if (serviceWorker != null)
            {
                // Update the status to "Approved"
                serviceWorker.Status = "Accepted";
                _context.SaveChanges();
            }
            return RedirectToAction("ManageServiceProviders");
        }
        [HttpPost]
        public IActionResult RejectServiceProvider(int userId, int serviceId)
        {
            // Find the ServiceWorkersJunctionTable entry
            var serviceWorker = _context.ServiceWorkersJunctionTables
                .FirstOrDefault(sw => sw.WrokerId == userId && sw.ServiceId == serviceId);
            if (serviceWorker != null)
            {
                // Update the status to "Rejected"
                serviceWorker.Status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("ManageServiceProviders");
        }
        public IActionResult ManageServiceProviders()
        {
            int ManagerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Step 1: Get the Location_Id for the current manager
            var managerLocationId = _context.Users
                .Where(u => u.UserId == ManagerId)
                .Select(u => u.LocationId)
                .FirstOrDefault();

            // Step 2: Get RoleID for 'Employee'
            var employeeRoleId = _context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstOrDefault();

            var employeesWithServices = (from u in _context.Users
                                         join ur in _context.UserRoles on u.UserId equals ur.UserId
                                         join sw in _context.ServiceWorkersJunctionTables on u.UserId equals sw.WrokerId into swj
                                         from sw in swj.DefaultIfEmpty()
                                         join s in _context.MainServices on sw.ServiceId equals s.ServiceId into sj
                                         from s in sj.DefaultIfEmpty()
                                         where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId 
                                         select new EmployeeWithServiceViewModel
                                         {
                                             UserId = u.UserId,
                                             FirstName = u.FirstName,
                                             LastName = u.LastName,
                                             Email = u.Email,
                                             CreatedAt = u.CreatedAt,
                                             PersonalImage = u.PersonalImage,
                                             PersonalAddress = u.PersonalAddress,
                                             DateOfBirth = u.DateOfBirth,
                                             PhoneNumber = u.PhoneNumber,
                                             Gender = u.Gender,
                                             LocationId = u.LocationId,
                                             WorkerServiceType = u.WorkerServiceType,
                                             WorkerRating = u.WorkerRating,
                                             WorkerIntro = u.WorkerIntro,
                                             IsActive = u.IsActive,
                                             ServiceName = s.ServiceName
                                         }).ToList();

            return View(employeesWithServices);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewEmployee(IFormCollection form, IFormFile? PersonalImage)
        {
            int managerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            var newUser = new User
            {
                FirstName = form["FirstName"],
                LastName = form["LastName"],
                Email = form["Email"],
                PasswordHash = form["Password"], // You should hash it securely
                PhoneNumber = form["PhoneNumber"],
                Gender = form["Gender"],
                PersonalAddress = form["PersonalAddress"],
                WorkerIntro = form["WorkerIntro"],
                DateOfBirth = string.IsNullOrWhiteSpace(form["DateOfBirth"]) ? null : DateOnly.Parse(form["DateOfBirth"]),
                CreatedAt = DateTime.Now,
                IsActive = true,
                LocationId = _context.Users.FirstOrDefault(u => u.UserId == managerId)?.LocationId
            };

            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using var ms = new MemoryStream();
                await PersonalImage.CopyToAsync(ms);
                newUser.PersonalImage = ms.ToArray();
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Assign "Employee" role
            var employeeRoleId = _context.Roles.FirstOrDefault(r => r.RoleName == "Employee")?.RoleId ?? 0;
            _context.UserRoles.Add(new UserRole { UserId = newUser.UserId, RoleId = employeeRoleId });
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageServiceProviders");
        }

        public IActionResult Edit_Employee_Info()
        {
            return RedirectToAction("ManageServiceProviders");
        }
        //        public IActionResult ManageServices()
        //        {
        //            return View();
        //        }
        public IActionResult AssginTask()
        {
            return View();
        }
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
