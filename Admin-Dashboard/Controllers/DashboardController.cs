using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
