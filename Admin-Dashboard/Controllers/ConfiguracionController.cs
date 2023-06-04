using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class ConfiguracionController : Controller
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
        public IActionResult Activos()
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
        public IActionResult ExportarDatos()
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
        public IActionResult GenerarLink()
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
