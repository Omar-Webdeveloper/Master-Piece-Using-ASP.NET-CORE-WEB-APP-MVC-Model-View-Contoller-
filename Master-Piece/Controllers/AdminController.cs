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
            return View();
        }
        public IActionResult show_Locations()
        {
            var viewModel = new LocationAreaViewModel
            {
                ouelocations = _context.LocationAreas.ToList(),
                OneLocation = new LocationArea() // For adding/editing a location
            };

            return View(viewModel);
        }
        public IActionResult AddLocation()
        {
            // Join tables: Users -> UserRoles -> Roles
            var managers = _context.Users
                .Join(_context.UserRoles, u => u.UserId, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, ur => ur.ur.RoleId, r => r.RoleId, (ur, r) => new { ur.u, r })
                .Where(userRole => userRole.r.RoleName == "Manager") // ✅ Only select Managers
                .Select(userRole => new SelectListItem
                {
                    Value = userRole.u.UserId.ToString(),
                    Text = userRole.u.FirstName + " " + userRole.u.LastName
                })
                .ToList();

            var viewModel = new LocationAreaViewModel
            {
                ouelocations = _context.LocationAreas.ToList(),
                OneLocation = new LocationArea()
            };

            ViewBag.Managers = managers; // ✅ Send list to ViewBag

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationAreaViewModel model)
        {


            var newLocation = new LocationArea
            {
                AreasCovered = model.OneLocation.AreasCovered,
                ManagerId = model.OneLocation.ManagerId // ✅ Assign the selected Manager ID
            };

            _context.LocationAreas.Add(newLocation);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Locations"); // ✅ Redirect back to the location list
        }
        public IActionResult EditLocation(int id)
        {
            var location = _context.LocationAreas
                .Include(l => l.Manager) // ✅ Include Manager details
                .FirstOrDefault(l => l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            // Fetch all available managers to allow selecting a different one
            var managers = _context.Users
                .Join(_context.UserRoles, u => u.UserId, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, ur => ur.ur.RoleId, r => r.RoleId, (ur, r) => new { ur.u, r })
                .Where(userRole => userRole.r.RoleName == "Manager")
                .Select(userRole => new SelectListItem
                {
                    Value = userRole.u.UserId.ToString(),
                    Text = userRole.u.FirstName + " " + userRole.u.LastName
                })
                .ToList();

            var viewModel = new LocationAreaViewModel
            {
                OneLocation = location,
                ouelocations = _context.LocationAreas.ToList()
            };

            ViewBag.Managers = managers; // ✅ Passing all managers to the view

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationAreaViewModel model)
        {
            var location = await _context.LocationAreas.FindAsync(model.OneLocation.LocationId);
            if (location == null)
            {
                return NotFound();
            }

            location.AreasCovered = model.OneLocation.AreasCovered;
            location.ManagerId = model.OneLocation.ManagerId; // ✅ Update Manager if changed

            _context.LocationAreas.Update(location);
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
                LocationId = model.AllManagers.LocationId, // ✅ Assign selected location
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
            var manager = await _context.Users.FindAsync(UserId);
            if (manager == null)
            {
                return NotFound();
            }

            manager.LocationId = LocationId; // ✅ Assign new location

            _context.Users.Update(manager);
            await _context.SaveChangesAsync();

            return RedirectToAction("show_Managers"); // ✅ Forces refresh
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
            return View();
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
