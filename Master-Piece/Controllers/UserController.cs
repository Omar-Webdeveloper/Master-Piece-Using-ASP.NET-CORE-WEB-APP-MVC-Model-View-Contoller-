using Microsoft.AspNetCore.Mvc;
using Master_Piece.Models;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        public IActionResult BookedServicesHistory()
        {
            return View();
        }
        public IActionResult Reset_Password()
        {
            return View();
        }
        public IActionResult Book_An_Appointmentwith_ThatWorker()
        {
            return View();
        }
        public IActionResult Show_Workers_for_Specfic_Service_()
        {
            return View();

        }
        public IActionResult Show_All_Services()
        {
            return View();
        }
    }
}
