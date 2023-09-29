using BusinessEntity.Request;
using BusinessEntity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class LoginController : Controller
    {
        private ValidationService _validationService;

        public LoginController(ValidationService validationService)
        {
            _validationService = validationService;

        }
        [HttpPost]
        public async Task RecuperarClave([FromBody]RequestRecuperarClave model)
        {
            try
            {
                //var RecuperarClave = await _validationService.RecuperarClave(model);

            }
            catch (Exception ex)
            {
            }
        }
    }
}
