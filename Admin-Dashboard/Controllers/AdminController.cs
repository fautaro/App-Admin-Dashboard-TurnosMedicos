using BusinessEntity.Services;
using DataAccess.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TuNamespace.Attributes;

namespace Admin_Dashboard.Controllers
{
    [AuthorizeOrRedirect]
    public class AdminController : Controller
    {
        private AdminService _adminService;
        private readonly UserManager<IdentityUser> _userManager;


        public AdminController(UserManager<IdentityUser> userManager, AdminService adminService)
        {
            _userManager = userManager;
            _adminService = adminService; 
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _adminService.ValidateAdmin(user.Id))
            {
                var response = await _adminService.GetAdminDashboard();
                return View(response);

            }
            return View();
        }

        public async Task<IActionResult> AgregarUsuario(string userId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarNuevoUsuario()
        {
            return View();
        }
    }
}
