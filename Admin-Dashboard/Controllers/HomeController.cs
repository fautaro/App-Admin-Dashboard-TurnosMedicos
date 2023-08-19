using Admin_Dashboard.Models;
using BusinessEntity.Response;
using BusinessEntity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TuNamespace.Attributes;

namespace Admin_Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DashboardService _dashboardService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, DashboardService dashboardService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _dashboardService = dashboardService;
            _userManager = userManager;

        }

        [AuthorizeOrRedirect]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var response = await _dashboardService.GetItems(user.Id);

            return View(response);
        }


        public IActionResult Privacy()
        {
            return View();
        }



        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}