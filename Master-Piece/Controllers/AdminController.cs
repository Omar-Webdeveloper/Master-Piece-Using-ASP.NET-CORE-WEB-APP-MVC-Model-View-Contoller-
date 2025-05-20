using Master_Piece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Admin_Dashboard()
        {
            int adminid = (int)HttpContext.Session.GetInt32("UserID");

            var adminName = _context.Users
                .Where(u => u.UserId == adminid)
                .Select(u => u.FirstName + " " + u.LastName)
                .FirstOrDefault();

            // Total Users with Role = "User"
            var totalUsers = (from ur in _context.UserRoles
                              join r in _context.Roles on ur.RoleId equals r.RoleId
                              where r.RoleName == "User"
                              select ur.UserId).Distinct().Count();

            // Total Employees with Role = "Employee"
            var totalEmployees = (from ur in _context.UserRoles
                                  join r in _context.Roles on ur.RoleId equals r.RoleId
                                  where r.RoleName == "Employee"
                                  select ur.UserId).Distinct().Count();

            // ✅ Total Employees with Role = "Employee" and status = "Accepted"
            var acceptedEmployeeIds = _context.ServiceWorkersJunctionTables
                .Where(sw => sw.Status == "Accepted")
                .Select(sw => sw.WrokerId)
                .Distinct()
                .ToList();

            var employeeRoleId = _context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstOrDefault();

            var acceptedEmployeeCount = (from ur in _context.UserRoles
                                         where ur.RoleId == employeeRoleId && acceptedEmployeeIds.Contains(ur.UserId)
                                         select ur.UserId).Distinct().Count();

            // Total Managers with Role = "Manager"
            var totalManagers = (from ur in _context.UserRoles
                                 join r in _context.Roles on ur.RoleId equals r.RoleId
                                 where r.RoleName == "Manager"
                                 select ur.UserId).Distinct().Count();

            // Covered Locations
            var coveredLocations = _context.LocationAreas
                .Where(l => !string.IsNullOrEmpty(l.AreasCovered))
                .Select(l => l.AreasCovered)
                .Distinct()
                .ToList();

            // Assign to ViewBag
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalEmployeesRequests = totalEmployees;
            ViewBag.AcceptedEmployees = acceptedEmployeeCount; // ✅ new ViewBag item
            ViewBag.TotalManagers = totalManagers;
            ViewBag.CoveredLocationCount = coveredLocations.Count;
            ViewBag.CoveredLocationList = coveredLocations;
            ViewBag.AdminName = adminName;

            return View();
        }

        public IActionResult show_All_Users() 
        {
            var users = (from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.RoleId
                         join u in _context.Users on ur.UserId equals u.UserId
                         where r.RoleName == "User"
                         select u).ToList();

            var vm = new AllManagersViewModel
            {
                All_Users = users
            };

            return View(vm);
        }
        public IActionResult show_All_Enployees() 
        {
            var employeeRoleId = _context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstOrDefault();

            // Get all employees with accepted service assignments
            var acceptedEmployeeIds = _context.ServiceWorkersJunctionTables
                .Where(sw => sw.Status == "Accepted")
                .Select(sw => sw.WrokerId)
                .Distinct()
                .ToList();

            // Now filter actual employee users based on role and accepted status
            var acceptedEmployees = (from ur in _context.UserRoles
                                     join u in _context.Users on ur.UserId equals u.UserId
                                     where ur.RoleId == employeeRoleId && acceptedEmployeeIds.Contains(u.UserId)
                                     select u).ToList();

            var vm = new AllManagersViewModel
            {
                All_Employees = acceptedEmployees
            };

            return View(vm);

        }

        public IActionResult show_Locations()
        {
            var locationsWithManagers = (from loc in _context.LocationAreas
                                         join user in _context.Users on loc.ManagerId equals user.UserId
                                         join ur in _context.UserRoles on user.UserId equals ur.UserId
                                         join role in _context.Roles on ur.RoleId equals role.RoleId
                                         where role.RoleName == "Manager"
                                         select new LocationWithManager
                                         {
                                             LocationId = loc.LocationId,
                                             AreasCovered = loc.AreasCovered,
                                             ManagerName = user.FirstName + " " + user.LastName
                                         }).ToList();
            // NEW: Get locations with no assigned manager
            var locationsWithoutManagers = _context.LocationAreas
                .Where(loc => loc.ManagerId == null)
                .Select(loc => new LocationDto
                {
                    LocationId = loc.LocationId,
                    AreasCovered = loc.AreasCovered
                }).ToList();
            var viewModel = new LocationAreaViewModel
            {
                LocationsWithoutManagers = locationsWithoutManagers, // ✅ Add this line
                LocationsWithManagers = locationsWithManagers,
                OneLocation = new LocationArea()
            };

            return View(viewModel);
        }

        public IActionResult AddLocation()
        {
            // Get only available managers (not assigned to any location)
            var availableManagers = _context.Users
                .Where(u => (u.LocationId == null || u.LocationId == 0)) // ✅ Only unassigned managers
                .Join(_context.UserRoles, u => u.UserId, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, ur => ur.ur.RoleId, r => r.RoleId, (ur, r) => new { ur.u, r })
                .Where(userRole => userRole.r.RoleName == "Manager")
                .Select(userRole => new SelectListItem
                {
                    Value = userRole.u.UserId.ToString(),
                    Text = userRole.u.FirstName + " " + userRole.u.LastName
                })
                .ToList();

            ViewBag.Managers = availableManagers;

            // Just initialize an empty LocationArea for the form
            var viewModel = new LocationAreaViewModel
            {
                OneLocation = new LocationArea()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationAreaViewModel model)
        {
            // Create the new location entry
            var newLocation = new LocationArea
            {
                AreasCovered = model.OneLocation.AreasCovered,
                ManagerId = model.OneLocation.ManagerId
            };

            _context.LocationAreas.Add(newLocation);
            await _context.SaveChangesAsync(); // Save first to generate LocationId (if using Identity column)

            // ✅ Update the manager's LocationId in the Users table
            var manager = await _context.Users.FindAsync(model.OneLocation.ManagerId);
            if (manager != null)
            {
                manager.LocationId = newLocation.LocationId; // Set the newly created location's ID
                _context.Users.Update(manager);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("show_Locations");
        }

        public IActionResult EditLocation(int id)
        {
            var location = _context.LocationAreas
                .Include(l => l.Manager) // ✅ Include Manager navigation property
                .FirstOrDefault(l => l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            // ✅ Get all available managers (LocationId is null or 0), or already assigned to THIS location
            var managers = _context.Users
                .Where(u =>
                    _context.UserRoles.Any(ur => ur.UserId == u.UserId && _context.Roles.Any(r => r.RoleId == ur.RoleId && r.RoleName == "Manager"))
                    &&
                    (u.LocationId == null || u.LocationId == 0 || u.LocationId == location.LocationId)
                )
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FirstName + " " + u.LastName
                })
                .ToList();

            var viewModel = new LocationAreaViewModel
            {
                OneLocation = location
            };

            ViewBag.Managers = managers;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationAreaViewModel model)
        {
            var location = await _context.LocationAreas
                .Include(l => l.Manager) // Include current manager
                .FirstOrDefaultAsync(l => l.LocationId == model.OneLocation.LocationId);

            if (location == null)
            {
                return NotFound();
            }

            // ✅ Get the current and new manager IDs
            var oldManagerId = location.ManagerId;
            var newManagerId = model.OneLocation.ManagerId;

            // ✅ Update location details
            location.AreasCovered = model.OneLocation.AreasCovered;
            location.ManagerId = newManagerId;

            // ✅ Update LocationArea
            _context.LocationAreas.Update(location);

            // ✅ Clear old manager’s LocationId (if different from new)
            if (oldManagerId != null && oldManagerId != newManagerId)
            {
                var oldManager = await _context.Users.FindAsync(oldManagerId);
                if (oldManager != null)
                {
                    oldManager.LocationId = null;
                    _context.Users.Update(oldManager);
                }
            }

            // ✅ Set new manager’s LocationId
            if (newManagerId != null)
            {
                var newManager = await _context.Users.FindAsync(newManagerId);
                if (newManager != null)
                {
                    newManager.LocationId = location.LocationId;
                    _context.Users.Update(newManager);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("show_Locations");
        }


        // POST: Delete Location
        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.LocationAreas.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.LocationAreas.Remove(location);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Locations");
        }

        public IActionResult show_Managers()
        {
            var managers = _context.Users
                .Join(_context.UserRoles, u => u.UserId, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, ur => ur.ur.RoleId, r => r.RoleId, (ur, r) => new { ur.u, r })
                .Where(userRole => userRole.r.RoleName == "Manager") // ✅ Select only Managers
                .Select(userRole => userRole.u) // ✅ Select only the User object
                .ToList();
            var locations = _context.LocationAreas.ToList();
            var viewModel = new AllManagersViewModel
            {
                Managers = managers, // ✅ Store managers in ViewModel
                Locations = locations
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Manager(AllManagersViewModel model, IFormFile PersonalImage)
        {


            byte[]? imageData = null;
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await PersonalImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            var newManager = new User
            {
                FirstName = model.AllManagers.FirstName,
                LastName = model.AllManagers.LastName,
                Email = model.AllManagers.Email,
                PasswordHash = model.AllManagers.PasswordHash,
                PersonalAddress = model.AllManagers.PersonalAddress,
                PhoneNumber = model.AllManagers.PhoneNumber,
                Gender = model.AllManagers.Gender,
                DateOfBirth = model.AllManagers.DateOfBirth,
                PersonalImage = imageData, // ✅ Store profile image
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newManager);
            await _context.SaveChangesAsync();

            // Assign Manager Role
            var managerRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Manager");
            if (managerRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = newManager.UserId,
                    RoleId = managerRole.RoleId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("show_Managers");
        }
        [HttpPost]
        public async Task<IActionResult> Update_Manager_Location(int UserId, int LocationId)
        {
            // 1. Get the new manager
            var newManager = await _context.Users.FindAsync(UserId);
            if (newManager == null)
            {
                return NotFound();
            }

            // 2. Find and unassign the old location of the new manager (if any)
            var oldLocation = await _context.LocationAreas
                .FirstOrDefaultAsync(l => l.ManagerId == UserId);
            if (oldLocation != null)
            {
                oldLocation.ManagerId = null;
                _context.LocationAreas.Update(oldLocation);
            }

            // 3. Find the new location and check if it was managed by another manager
            var newLocation = await _context.LocationAreas
                .FirstOrDefaultAsync(l => l.LocationId == LocationId);
            if (newLocation != null)
            {
                // If a different manager was managing this location, remove their LocationId
                if (newLocation.ManagerId != null && newLocation.ManagerId != UserId)
                {
                    var oldManager = await _context.Users.FindAsync(newLocation.ManagerId);
                    if (oldManager != null)
                    {
                        oldManager.LocationId = null;
                        _context.Users.Update(oldManager);
                    }
                }

                // Assign the new manager to the location
                newLocation.ManagerId = UserId;
                _context.LocationAreas.Update(newLocation);
            }

            // 4. Update the new manager's LocationId
            newManager.LocationId = LocationId;
            _context.Users.Update(newManager);

            // 5. Save all changes
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Managers");
        }
                                                      // Delete Manager Action
            [HttpPost]
        public async Task<IActionResult> Delete_Manager(int id)
        {
            var manager = await _context.Users.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Users.Remove(manager);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Managers");
        }

        public IActionResult show_Services()
        {
            var listservices = _context.MainServices.ToList();
            return View(listservices);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Service(MainService service, IFormFile ServiceImage)
        {
            byte[]? imageData = null;
            if (ServiceImage != null && ServiceImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ServiceImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            service.Image = imageData;

            _context.MainServices.Add(service);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Services");
        }
        [HttpPost]
        public async Task<IActionResult> Update_Service(MainService updatedService, IFormFile ServiceImage)
        {
            var existingService = await _context.MainServices.FindAsync(updatedService.ServiceId);
            if (existingService == null)
            {
                return NotFound();
            }

            existingService.ServiceName = updatedService.ServiceName;
            existingService.Description = updatedService.Description;
            existingService.ServicePrice = updatedService.ServicePrice;

            // Handle Image Upload
            if (ServiceImage != null && ServiceImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ServiceImage.CopyToAsync(memoryStream);
                    existingService.Image = memoryStream.ToArray(); // ✅ Save new image
                }
            }

            _context.MainServices.Update(existingService);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Services");
        }
        [HttpPost]
        public async Task<IActionResult> Delete_Service(int ServiceId)
        {
            var service = await _context.MainServices
                .Include(s => s.Bookings)
                .ThenInclude(b => b.Reviews)  // ✅ Load related reviews
                .Include(s => s.Bookings)
                .ThenInclude(b => b.Payment)  // ✅ Load related payments
                .FirstOrDefaultAsync(s => s.ServiceId == ServiceId);

            if (service == null)
            {
                return NotFound();
            }

            // ✅ Remove related reviews and payments first
            foreach (var booking in service.Bookings)
            {
                _context.Reviews.RemoveRange(booking.Reviews);
                _context.Payments.RemoveRange(booking.Payment);
            }

            _context.Bookings.RemoveRange(service.Bookings);
            _context.MainServices.Remove(service);

            await _context.SaveChangesAsync();

            return RedirectToAction("show_Services");
        }
        public IActionResult Contact_Us_Messsages()
        {
            var contact = _context.ContactUs.ToList();

            return View(contact);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteContact(int contactid)
        {
            // 1. Find the contact message
            var contactMessage = await _context.ContactUs.FindAsync(contactid);
            if (contactMessage == null)
            {
                return NotFound();
            }

            // 2. Remove the contact message
            _context.ContactUs.Remove(contactMessage);

            // 3. Save changes
            await _context.SaveChangesAsync();

            // 4. Redirect back to list
            return RedirectToAction("Contact_Us_Messsages");
        }
        public IActionResult Check_Review_Status()
        {
            var Show_My_Services = _context.MainServices // Limit to 4 services
                                              .Select(s => new MainService
                                              {
                                                  ServiceId = s.ServiceId,
                                                  ServiceName = s.ServiceName,
                                                  Description = s.Description,
                                                  Image = s.Image
                                              }).ToList();


            var randomReviews = (from r in _context.Reviews
                                 join b in _context.Bookings on r.BookingId equals b.BookingId
                                 join u in _context.Users on b.UserId equals u.UserId
                                 join w in _context.Users on b.WorkerId equals w.UserId
                                 join s in _context.MainServices on b.ServiceId equals s.ServiceId
                                 select new ReviewViewModel
                                 {
                                     ReviewId = r.ReviewId,
                                     Rating = (int)r.Rating,
                                     Comment = r.Comment,
                                     ReviewStatus = r.ReviewStatus,
                                     CreatedAt = r.CreatedAt,
                                     ImageWhereTheIssueLocated = b.ImageWhereTheIssueLocated,
                                     ImageAfterFixing = b.ImageAfterFixing,
                                     UserFirstName = u.FirstName,
                                     WorkerName = w.FirstName + " " + w.LastName,
                                     ServiceName = s.ServiceName
                                 }).ToList();




            var model = new HomePageViewModel
            {
                MainServices = Show_My_Services.Select(s => new MainServiceViewModel
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    Image = s.Image
                }).ToList(),

                RandomReviews = randomReviews,

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AcceptReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.ReviewStatus = "Accepted_To_Show_All"; // Or whatever your status column is
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Check_Review_Status", "Admin"); // Adjust if needed
        }
        [HttpPost]
        public IActionResult TransferToManager(int reviewId)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review != null)
            {
                review.ReviewStatus = "Rejected_Transfer_To_Manager";
                _context.SaveChanges();
            }

            return RedirectToAction("Check_Review_Status");
        }
        [HttpPost]
        public IActionResult SetNeutralReview(int reviewId)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review != null)
            {
                review.ReviewStatus = "Netural";
                _context.SaveChanges();
            }

            return RedirectToAction("Check_Review_Status");
        }


        public IActionResult AdminProfile()
        {
            // Retrieve user Id from the session
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);


            return View(user); // Pass the User object to the view
        }
        [HttpGet]
        public IActionResult AdminEditProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult AdminEditProfile(User updatedUser, IFormFile PersonalImage)
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

            return RedirectToAction("AdminProfile");
        }


        public IActionResult AdminResetPassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult AdminResetPassword(string OldPassword, string NewPassword, string ConfirmNewPassword)
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
                    return RedirectToAction("AdminProfile");
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
