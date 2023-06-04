using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class AccionesController : Controller
    {

        public IActionResult TurnosConfirmados()
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

        public IActionResult AltaManual()
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

        public IActionResult AdministrarHoras()
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

        public IActionResult ExportarCalendar()
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
        public IActionResult ExportarExcel()
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
