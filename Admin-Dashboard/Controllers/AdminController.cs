﻿using BusinessEntity.Request;
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
        //Paso 2: Registrar datos de usuario
        public async Task<IActionResult> AgregarUsuario(string userId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _adminService.ValidateAdmin(user.Id))
            {
                var response = await _adminService.GetDatosNuevoUsuario(userId);
                return View(response);

            }
            return View();
        }

        //Paso 2: Registrar datos de usuario (POST)
        [HttpPost]
        public async Task<bool> GuardarDatosNuevoUsuario([FromBody]RequestGuardarDatosNuevoUsuario request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (await _adminService.ValidateAdmin(user.Id))
            {
                var response = await _adminService.GuardarDatosNuevoUsuario(request);
                return true;

            }
            return false;
        }

        public async Task<IActionResult> ValidateAdmin()
        {

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _adminService.ValidateAdmin(user.Id);
            return PartialView("isAdmin", isAdmin);

        }

        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _adminService.ValidateAdmin(user.Id))
            {
                var response = await _adminService.GetEdit(id);
                return View(response);

            }
            return View();

        }

    }
}
