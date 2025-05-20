using System.Diagnostics;
using Master_Piece.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Master_Piece.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var Show_My_Services = _context.MainServices.OrderBy(r => Guid.NewGuid()) // Randomize the order
                                              .Take(4) // Limit to 4 services
                                              .Select(s => new MainService
                                              {
                                                  ServiceId = s.ServiceId,
                                                  ServiceName = s.ServiceName,
                                                  Description = s.Description,
                                                  Image = s.Image
                                              }).ToList();


            var randomReviews = (from r in _context.Reviews
                                 where r.ReviewStatus == "Accepted_To_Show_All"
                                 join b in _context.Bookings on r.BookingId equals b.BookingId
                                 join u in _context.Users on b.UserId equals u.UserId
                                 orderby Guid.NewGuid() // Randomize the order
                                 select new ReviewViewModel
                                 {
                                     Rating = (int)r.Rating,
                                     Comment = r.Comment,
                                     CreatedAt = r.CreatedAt,
                                     ImageWhereTheIssueLocated = b.ImageWhereTheIssueLocated,
                                     ImageAfterFixing = b.ImageAfterFixing,
                                     UserFirstName = u.FirstName
                                 }).Take(4).ToList();



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

                // Initialize the ContactUs form as an empty object
                ContactUsForm = new ContactUs()
            };

            return View(model);
        }

        public IActionResult About_US()
        {
            var Show_My_Services = _context.MainServices.OrderBy(r => Guid.NewGuid()) // Randomize the order
                                              .Select(s => new MainService
                                              {
                                                  ServiceName = s.ServiceName,
                                                  Image = s.Image,
                                                  ServiceId = s.ServiceId
                                              }).ToList();
            return View(Show_My_Services);
        }
        public IActionResult Contact_US()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitContactfromhomepage(HomePageViewModel  model)
        {
            if (ModelState.IsValid)
            {
                // Extract the ContactUsForm from the model
                var contactUs = model.ContactUsForm;

                // Save the ContactUsForm data to the database
                // For example, insert into the ContactUs table (adjust based on your DB context and model)
                var contactUsEntry = new ContactU
                {
                    FirstName = contactUs.FirstName,
                    LastName = contactUs.LastName,
                    Email = contactUs.Email,
                    ContactUsMessage = contactUs.ContactUsMessage,
                };
                _context.ContactUs.Add(contactUsEntry);
                _context.SaveChanges();

                // Add success message to TempData
                TempData["SuccessMessage"] = "Thank you for contacting us! We will get back to you shortly.";

                return RedirectToAction("Index"); // Redirect to the home page or any page
            }
            TempData["ErrorMessage"] = "Something went wrong!Please Try Again";

            return RedirectToAction("Index"); // Return to the form if validation fails
        }
        [HttpPost]
        public IActionResult SubmitContact(ContactU contact)
        {
            if (ModelState.IsValid)
            {
                _context.ContactUs.Add(contact);
                _context.SaveChanges();
                // Add success message to TempData
                TempData["SuccessMessage"] = "Thank you for contacting us! We will get back to you shortly.";
                return RedirectToAction("Index"); // Redirect to a "Thank You" page after submission
            }
            TempData["ErrorMessage"] = "Something went wrong!Please Try Again";

            return RedirectToAction("Contact_US"); // Return to the form if validation fails
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid email or password."; // ?? Set error for SweetAlert
                return View(model); // Return the same view with validation errors
            }

            // Step 1: Check email and password
            var Loginned_user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.PasswordHash);

            if (Loginned_user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                TempData["ErrorMessage"] = "Invalid email or password."; // ?? Set error for SweetAlert

                return View(model);
            }

            // Step 2: Save UserID in session
            HttpContext.Session.SetInt32("UserID", Loginned_user.UserId);


            // Step 3: Retrieve UserRole and Role
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == Loginned_user.UserId);
            if (userRole == null)
            {
                ModelState.AddModelError("", "User role not found.");
                return View(model);
            }

            HttpContext.Session.SetInt32("RoleID", (int)userRole.RoleId);

            var role = _context.Roles.FirstOrDefault(r => r.RoleId == userRole.RoleId);
            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist.");
                return View(model);
            }
            // Step 4: Set TempData success message
            TempData["SuccessMessage"] = "You have logged in successfully!"; // ?? SUCCESS here
            HttpContext.Session.SetString("UserRole", role.RoleName);

            // Step 4: Redirect based on Role
            return role.RoleName switch
            {
                "Guest" => RedirectToAction("Login", "Home"),
                "User" => RedirectToAction("Index", "Home"),
                "Employee" => RedirectToAction("Employee_Dashboard", "Employee"),
                "Manager" => RedirectToAction("Manager_Dashboard", "Manager"),
                "Admin" => RedirectToAction("Admin_Dashboard", "Admin"),
                "SuperAdmin" => RedirectToAction("SuperAdmin_Dashboard", "SuperAdmin"),
                _ => RedirectToAction("Login", "Home")
            };
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register_New_UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again."; // ?? Set error for SweetAlert
                return View(model);
            }

            if (model.PasswordHash != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                TempData["ErrorMessage"] = "ConfirmPassword and Passwords do not match."; // ?? Set error for SweetAlert
                return View(model);
            }

                // Load default image from wwwroot
                byte[] imageBytes;
                if (model.PersonalImage != null && model.PersonalImage.Length > 0)
                {
                    // Convert uploaded image to byte[]
                    using (var memoryStream = new MemoryStream())
                    {
                        model.PersonalImage.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
                else
                {
                    // Load default image from wwwroot (if you still want to support this case)
                    string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Guest_User_Image.jpg");
                    imageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
                }

                // Create a new User entity and map from ViewModel
                var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                PersonalImage = imageBytes,
                    PersonalAddress = model.PersonalAddress,
                    DateOfBirth = model.DateOfBirth,
                    Gender= model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    WorkerServiceType = model.WorkerServiceType,
                    CreatedAt = DateTime.Now,
                    IsActive = true


                };

            // Add user to database
            _context.Users.Add(user);
            _context.SaveChanges();


            // Assign "User" role
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == "User");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                });

                _context.SaveChanges();
            }
            // ? Set success TempData
            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                // Optional: log the exception using ILogger or to a file/db
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }


        public IActionResult Forget_Password()
        {
            return View();
        }
        public IActionResult Register_New_Employee()
        {
            var viewModel = new Register_New_EmployeeViewModel
            {
                ServicesList = _context.MainServices.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.ServiceName
                }).ToList(),

                LocationsList = _context.LocationAreas.Select(l => new SelectListItem
                {
                    Value = l.LocationId.ToString(),
                    Text = l.AreasCovered
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register_New_Employee(Register_New_EmployeeViewModel model, IFormFile PersonalImage)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Register_New_Employee", "Home");
            }

            // Map ViewModel to User entity
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                PersonalAddress = model.PersonalAddress,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                WorkerServiceType = model.WorkerServiceType,
                LocationId = model.SelectedLocationId
            };

            // Handle image upload
            if (PersonalImage != null && PersonalImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await PersonalImage.CopyToAsync(ms);
                    user.PersonalImage = ms.ToArray();
                }
            }
            else
            {
                // Optionally set a default image here
                string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Guest_User_Image.jpg");
                if (System.IO.File.Exists(defaultImagePath))
                {
                    user.PersonalImage = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                }
            }

            // Save User
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Save Role
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == "Employee");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                });
                await _context.SaveChangesAsync();
            }
            // ? Save to ServiceWorkersJunctionTable
            if (model.SelectedServiceId.HasValue)
            {
                _context.ServiceWorkersJunctionTables.Add(new ServiceWorkersJunctionTable
                {
                    WrokerId = user.UserId,
                    ServiceId = model.SelectedServiceId.Value
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Login", "Home");
        }


        public IActionResult Testmonials()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            HttpContext.Session.Remove("Role");
            Response.Cookies.Delete(".AspNetCore.Session");
            TempData["SuccessMessage"] = "You have successfully logged out. See you next time!";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult test()
        {
            var teting=_context.MainServices.ToList();
            return View(teting);
        }
        [HttpPost]
        public IActionResult submittest(int serviceId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    var service = _context.MainServices.FirstOrDefault(s => s.ServiceId == serviceId);
                    if (service != null)
                    {
                        service.Image = fileBytes; // <-- Save the file as byte array
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("test");
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
