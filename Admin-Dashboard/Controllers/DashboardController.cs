using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // El usuario está autenticado, puedes realizar acciones específicas para usuarios autenticados
                return View();
            }
            else
            {
                return Redirect("~/login");
            }
        }
    }
}
