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




            // Get employees under the same location
            var employeeIds = (from u in _context.Users
                               join ur in _context.UserRoles on u.UserId equals ur.UserId
                               where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId
                               select u.UserId).ToList();

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

            // Get completed bookings grouped by day
            var completedJobs = _context.Bookings
                .Where(b => employeeIds.Contains(b.WorkerId ?? 0)
                    && b.Status == "Completed"
                    && b.BookingEndDate.HasValue
                    && b.BookingEndDate.Value.Month == currentMonth
                    && b.BookingEndDate.Value.Year == currentYear)
                .GroupBy(b => b.BookingEndDate.Value.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Day, g => g.Count);

            // Prepare data for the chart
            ViewBag.ChartLabels = Enumerable.Range(1, daysInMonth)
                .Select(day => day.ToString())
                .ToArray();

            ViewBag.ChartData = Enumerable.Range(1, daysInMonth)
                .Select(day => completedJobs.ContainsKey(day) ? completedJobs[day] : 0)
                .ToArray();



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
            return RedirectToAction("Manage_New_ServiceProviders");
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
                                         where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId && sw.Status == "Accepted"
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
            var services = _context.MainServices.ToList();

            var locations = _context.LocationAreas
                .Where(l => l.ManagerId == ManagerId)
                .ToList();
            var viewModel = new ManageEmployeesPageViewModel
            {
                EmployeesWithServices = employeesWithServices,
                Services = services,
                Locations = locations
            };
            // If you want to pass the employee data as well, create a new wrapper class:
            ViewBag.EmployeesWithServices = employeesWithServices;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewEmployee(IFormCollection form, IFormFile? PersonalImage)
        {
            int managerId = HttpContext.Session.GetInt32("UserID") ?? 0;

            // Parse form values
            int.TryParse(form["LocationId"], out int locationId);
            int.TryParse(form["ServiceId"], out int serviceId);

            var newUser = new User
            {
                FirstName = form["FirstName"],
                LastName = form["LastName"],
                Email = form["Email"],
                PasswordHash = form["Password"], // Ideally hash the password
                PhoneNumber = form["PhoneNumber"],
                Gender = form["Gender"],
                PersonalAddress = form["PersonalAddress"],
                WorkerIntro = form["WorkerIntro"],
                DateOfBirth = string.IsNullOrWhiteSpace(form["DateOfBirth"])
                              ? null
                              : DateOnly.Parse(form["DateOfBirth"]),
                WorkerRating = string.IsNullOrWhiteSpace(form["WorkerRating"])
                               ? null
                               : double.Parse(form["WorkerRating"]),
                CreatedAt = DateTime.Now,
                IsActive = true,
                LocationId = locationId // <-- use selected LocationId from form
            };

            // Handle image upload
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using var ms = new MemoryStream();
                await PersonalImage.CopyToAsync(ms);
                newUser.PersonalImage = ms.ToArray();
            }

            // Save user
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Assign "Employee" role
            var employeeRoleId = _context.Roles.FirstOrDefault(r => r.RoleName == "Employee")?.RoleId ?? 0;
            _context.UserRoles.Add(new UserRole
            {
                UserId = newUser.UserId,
                RoleId = employeeRoleId
            });
            await _context.SaveChangesAsync();

            // Add entry to ServiceWorkersJunctionTable
            var serviceLink = new ServiceWorkersJunctionTable
            {
                WrokerId = newUser.UserId,
                ServiceId = serviceId,
                Status = "Accepted" // or "Accepted" depending on your logic
            };
            _context.ServiceWorkersJunctionTables.Add(serviceLink);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageServiceProviders");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeInfo(int UserId, string WorkerIntro, double WorkerRating)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user != null)
            {
                user.WorkerIntro = WorkerIntro;
                user.WorkerRating = WorkerRating;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageServiceProviders");
        }
        [HttpPost]
        public IActionResult SubmitEvaluation(IFormCollection form)
        {
            try
            {
                // Parse the WorkerId
                int workerId = int.Parse(form["WorkerId"]);

                // Sum the answers from Q1 to Q10
                double totalScore = 0;
                for (int i = 1; i <= 10; i++)
                {
                    string key = $"Q{i}";
                    if (form.ContainsKey(key) && double.TryParse(form[key], out double score))
                    {
                        totalScore += score;
                    }
                }

                // Get the comment (optional)
                string comments = form["Comments"];

                // Create the Evaluation record
                Evaluation evaluation = new Evaluation
                {
                    WrokerId = workerId,
                    Score = totalScore,
                    Comments = string.IsNullOrWhiteSpace(comments) ? null : comments,
                    CreatedAt = DateTime.Now,
                    EvaluationYear = DateTime.Now.Year
                };

                // Save to DB
                _context.Evaluations.Add(evaluation);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Evaluation submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error submitting the evaluation.";
                // Log error (optional): _logger.LogError(ex, "Evaluation failed.");
            }

            return RedirectToAction("ManageServiceProviders"); // Change to your actual return view
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAgainEmployeeInfo(UpdateEmployeeInfoViewModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound();

            // Update intro and rating
            user.WorkerRating = model.WorkerRating;
            user.WorkerIntro = model.WorkerIntro;

            // Update location
            user.LocationId = model.LocationId;

            // Update the service junction table
            var existingLink = await _context.ServiceWorkersJunctionTables
                .FirstOrDefaultAsync(es => es.WrokerId == model.UserId);

            if (existingLink != null)
            {
                existingLink.ServiceId = model.ServiceId;
            }


            await _context.SaveChangesAsync();

            return RedirectToAction("ManageServiceProviders"); // or wherever you want
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int userId)
        {
            // 1. Find the user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 2. Remove related ServiceWorker junction records
            var serviceLinks = await _context.ServiceWorkersJunctionTables
                .Where(sw => sw.WrokerId == userId)
                .ToListAsync();
            if (serviceLinks.Any())
            {
                _context.ServiceWorkersJunctionTables.RemoveRange(serviceLinks);
            }

            // 3. Remove related UserRole
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            if (userRoles.Any())
            {
                _context.UserRoles.RemoveRange(userRoles);
            }

            // 4. (Optional) Remove related evaluations, tasks, etc.
            // Example: _context.Evaluations.RemoveRange(user.Evaluations);

            // 5. Remove the user
            _context.Users.Remove(user);

            // 6. Save all changes
            await _context.SaveChangesAsync();

            // 7. Redirect back to list
            return RedirectToAction("ManageServiceProviders");
        }


        public IActionResult AssginTask()
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
                                         where ur.RoleId == employeeRoleId && u.LocationId == managerLocationId && sw.Status == "Accepted"
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
            var services = _context.MainServices.ToList();

            var locations = _context.LocationAreas
                .Where(l => l.ManagerId == ManagerId)
                .ToList();
            var viewModel = new ManageEmployeesPageViewModel
            {
                EmployeesWithServices = employeesWithServices,
                Services = services,
                Locations = locations
            };
            // If you want to pass the employee data as well, create a new wrapper class:
            ViewBag.EmployeesWithServices = employeesWithServices;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AssignTaskToEmployees(string employeeIds, string TaskName, DateOnly StartDate, DateOnly EndDate, string TaskStatus, string TasksDetails, IFormFile BeforePhoto)
        {
            if (string.IsNullOrWhiteSpace(employeeIds))
                return BadRequest("No employees selected.");

            var employeeIdList = employeeIds.Split(',').Select(int.Parse).ToList();

            // Save task for each employee  
            foreach (var empId in employeeIdList)
            {
                var task = new Models.Task
                {
                    WrokerId = empId,
                    TaskName = TaskName,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    TaskStatus = TaskStatus,
                    TasksDetails = TasksDetails,
                    BeforePhoto = await ConvertToBytesAsync(BeforePhoto),
                };

                _context.Tasks.Add(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AssginTask"); // Or wherever you want to go after  
        }

        private async Task<byte[]> ConvertToBytesAsync(IFormFile file)
        {
            if (file == null) return null;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public IActionResult check_Bookings_Tasks_Status()
        {
            int? managerId = HttpContext.Session.GetInt32("UserID");
            if (managerId == null)
                return RedirectToAction("Login", "Home");

            // Get all employees managed by this manager
            var employeeIds = _context.Users
                .Where(u => u.Location.ManagerId == managerId)
                .Select(u => u.UserId)
                .ToList();

            // Get completed bookings assigned to these employees
            var completedBookings = _context.Bookings
                .Where(b => employeeIds.Contains((int)b.WorkerId) && b.Status == "Completed")
                .ToList();

            // Get completed tasks assigned to these employees
            var completedTasks = _context.Tasks
                .Where(t => employeeIds.Contains((int)t.WrokerId) && t.TaskStatus == "Completed")
                .ToList();

            var viewModel = new BookingsTasksStatusViewModel
            {
                CompletedBookings = completedBookings,
                CompletedTasks = completedTasks
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult SetBookingStatus(int bookingId, string status)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null && (status == "Pending" || status == "Confirmed" || status == "Completed"))
            {
                booking.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("check_Bookings_Tasks_Status");
        }
        [HttpPost]
        public IActionResult SetTaskStatus(int taskId, string status)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
            if (task != null && (status == "Pending" || status == "Confirmed" || status == "Completed"))
            {
                task.TaskStatus = status;
                _context.SaveChanges();
            }
            return RedirectToAction("check_Bookings_Tasks_Status");
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

    }
}
