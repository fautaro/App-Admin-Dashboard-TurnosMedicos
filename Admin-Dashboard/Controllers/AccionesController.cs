using BusinessEntity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TuNamespace.Attributes;

namespace Admin_Dashboard.Controllers
{
    [AuthorizeOrRedirect]

    public class AccionesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private ReservaService _reservaService;

        public AccionesController(UserManager<IdentityUser> userManager, ReservaService reservaService)
        {
            _userManager = userManager;
            _reservaService = reservaService;

        }
        public async Task<IActionResult> HorariosBloqueados()
        {
            return View();

        }

        public async Task<IActionResult> TurnosConfirmados()
        {


            var user = await _userManager.GetUserAsync(User);

            var response = await _reservaService.GetTurnosConfirmados(user.Id);


            return View(response);

        }
        [HttpPost]
        public async Task<bool> CancelarTurno(int id)
        {
            try
            {
                //if (string.IsNullOrEmpty(id)) return false;
                bool Success = false;

                Success = await _reservaService.CancelarTurno(id);
                return Success;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<IActionResult> ExportarIcs()
        {
            var user = await _userManager.GetUserAsync(User);

            var Response = await _reservaService.GenerarIcs(user.Id);

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Response));

            return File(memoryStream, "text/calendar", "eventos.ics");

        }

        public async Task<IActionResult> ExportarCalendar()
        {
            return View();

        }


        public IActionResult AltaManual()
        {
            return View();

        }

        public IActionResult AdministrarHoras()
        {
            return View();

        }


        public IActionResult ExportarExcel()
        {
            return View();

        }
    }
}
