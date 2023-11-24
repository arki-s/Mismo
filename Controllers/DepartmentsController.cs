using Microsoft.AspNetCore.Mvc;

namespace Mismo.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
