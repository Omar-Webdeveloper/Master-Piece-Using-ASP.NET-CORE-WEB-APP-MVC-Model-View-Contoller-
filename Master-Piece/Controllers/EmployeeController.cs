using Microsoft.AspNetCore.Mvc;

namespace Master_Piece.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Employee_Dashboard()
        {
            return View();
        }
    }
}
