using BusinessEntity.Response;
using DataAccess.Services;
using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Services
{
    public class AdminService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;
        private MailService _mailService;
        private readonly DateTime fechaActual = DateTime.Now;



        public AdminService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService, MailService mailService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;
            _mailService = mailService;
        }

        public async Task<ResponseGetUsuarios> GetAdminDashboard()
        {
            ResponseGetUsuarios response = new ResponseGetUsuarios();
            try
            {
                List<AdminDashboardResult> usuarios = await _dbWrapper.GetAdminDashboard();

                List<Usuario> usuariolist = new List<Usuario>();
                response.Success = true;


                foreach (var item in usuarios)
                {
                    Usuario usuario = new Usuario()
                    {
                        Id = item.Id,
                        Nombre = item.Nombre,
                        Apellido = item.Apellido,
                        Activo = item.Activo,
                        Email = item.Email,
                        Profesion = item.Profesion,
                        Profesional_Id = item.Profesional_Id
                    };

                    usuariolist.Add(usuario);
                }

                response.Usuarios = usuariolist;


                return response;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> ValidateAdmin(string AuthenticatedUser)
        {
            try
            {

                var user = await _dbWrapper.ValidateAdmin(AuthenticatedUser);

                if (user == null) return false;

                if (user.Administrador_Id == AuthenticatedUser)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
