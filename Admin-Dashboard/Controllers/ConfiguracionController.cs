using BusinessEntity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TuNamespace.Attributes;

namespace Admin_Dashboard.Controllers
{
    [AuthorizeOrRedirect]

    public class ConfiguracionController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private ReservaService _reservaService;
        private PerfilService _perfilService;


        public ConfiguracionController(ReservaService reservaService, PerfilService perfilService, UserManager<IdentityUser> userManager)
        {
            _reservaService = reservaService;
            _perfilService = perfilService;
            _userManager = userManager;

        }

        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);

            var response = await _perfilService.GetItemsPerfilReducido(user.Id, user.Email);
            
            return View(response);


        }

        public async Task<IActionResult> PerfilPublico()
        {
            var user = await _userManager.GetUserAsync(User);

            var response = await _perfilService.GetItemsPerfilPublico(user.Id, user.Email);

            return View(response);


        }
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
