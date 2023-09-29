using BusinessEntity.Request;
using BusinessEntity.Response;
using DataAccess.Models;
using DataAccess.Services;
using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public async Task<bool> GuardarDatosNuevoUsuario(RequestGuardarDatosNuevoUsuario request)
        {
            try
            {
                Profesional profesional = new Profesional();

                profesional.Titulo = request.titulo;
                profesional.Activo = true;
                profesional.Alias = request.alias;
                profesional.Nombre = request.nombre;
                profesional.Apellido = request.apellido;
                profesional.Descripcion = request.descripcion;
                profesional.Intervalo = Convert.ToInt32(request.intervalo);
                profesional.Profesion_Id = Convert.ToInt32(request.profesion_Id);


                var idNuevoUsuario = await _dbWrapper.CrearProfesional(profesional);

                if (idNuevoUsuario != null && idNuevoUsuario != 0)
                {
                    UsuarioProfesional Uprof = new UsuarioProfesional();
                    Uprof.User_Id = request.usuario_id;
                    Uprof.Profesional_Id = idNuevoUsuario;


                    var SuccessCrearRelUsuarioProfesional = await _dbWrapper.CrearRelUsuarioProfesional(Uprof);

                    if (SuccessCrearRelUsuarioProfesional)
                    {
                        List<TimeSpan> tiempos = new List<TimeSpan>();
                        foreach (string horario in request.horarios)
                        {
                            if (TimeSpan.TryParseExact(horario, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan resultado))
                            {
                                tiempos.Add(resultado);
                            }
                        }
                        List<Horario> horarios = new List<Horario>();

                        tiempos.Sort();
                        foreach (var item in tiempos)
                        {
                            Horario horario = new Horario()
                            {
                                Profesional_Id = idNuevoUsuario,
                                Hora = item
                            };
                            horarios.Add(horario);
                        }
                        var SuccessGuardarHorarios = await _dbWrapper.GuardarHorariosProfesional(horarios);

                        if (SuccessGuardarHorarios)
                        {
                            return true;
                        }
                    } 
                }
                return false;
            }
            catch (Exception ex)
            {
                await _dbWrapper.GuardarEvento("Admin", $"Error al crear usuario: {ex}", "");

                return false;
                throw;
            }
        }
        public async Task<ResponseGetDatosAgregarUsuario> GetDatosNuevoUsuario(string userId)
        {
            ResponseGetDatosAgregarUsuario response = new ResponseGetDatosAgregarUsuario();

            try
            {
                var Profesiones = await _dbWrapper.GetProfesionesAdmin();

                if (Profesiones != null)
                {
                    response.Profesion = new List<ProfesionViewModel>();

                    foreach (var item in Profesiones)
                    {
                        ProfesionViewModel profesion = new ProfesionViewModel()
                        {
                            Nombre = item.Nombre,
                            Profesion_Id = item.Profesion_Id,
                            Rubro = item.Rubro
                        };
                        response.Profesion.Add(profesion);
                    }
                }

                response.Usuario_Id = userId;
                response.Success = true;

                return response;

            }
            catch (Exception ex)
            {
                await _dbWrapper.GuardarEvento("Turno", $"Error al obtener datos de nuevo usuario: {ex}", "");

                response.Success = false;
                return response;
                throw;
            }

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

        public async Task<ResponseGetEdit> GetEdit(string User)
        {
            ResponseGetEdit response = new ResponseGetEdit();
            try
            {
                var UsuarioProfesional = await _dbWrapper.GetUsuarioProfesionalAdmin(User);
                var Profesional = await _dbWrapper.GetProfesionalAdmin(User, UsuarioProfesional);
                var Horarios = await _dbWrapper.GetUserHorariosAdmin(User, UsuarioProfesional);
                var Profesiones = await _dbWrapper.GetProfesionesAdmin();

                if (UsuarioProfesional != null)
                {
                    response.UsuarioProfesional = new UsuarioProfesionalViewModel()
                    {
                        Profesional_Id = UsuarioProfesional.Profesional_Id,
                        User_Id = UsuarioProfesional.User_Id,
                        UsuarioProfesional_Id = UsuarioProfesional.UsuarioProfesional_Id
                    };
                }

                if (Profesional != null)
                {
                    response.Profesional = new ProfesionalViewModel()
                    {
                        Activo = Profesional.Activo,
                        Alias = Profesional.Alias,
                        Apellido = Profesional.Apellido,
                        Descripcion = Profesional.Descripcion,
                        Imagen = Profesional.Imagen,
                        Intervalo = Profesional.Intervalo,
                        Profesional_Id = Profesional.Profesional_Id,
                        Nombre = Profesional.Nombre,
                        Profesion_Id = Profesional.Profesion_Id,
                        Titulo = Profesional.Titulo,
                        Usuario_Id = Profesional.Usuario_Id
                    };
                }

                if (Horarios != null)
                {
                    response.Horario = new List<HorarioViewModel>();
                    foreach (var item in Horarios)
                    {
                        HorarioViewModel horario = new HorarioViewModel()
                        {
                            Hora = item.Hora,
                            Horario_Id = item.Horario_Id,
                            Profesional_Id = item.Profesional_Id
                        };
                        response.Horario.Add(horario);

                    }
                }

                if (Profesiones != null)
                {
                    response.Profesion = new List<ProfesionViewModel>();

                    foreach (var item in Profesiones)
                    {
                        ProfesionViewModel profesion = new ProfesionViewModel()
                        {
                            Nombre = item.Nombre,
                            Profesion_Id = item.Profesion_Id,
                            Rubro = item.Rubro
                        };
                        response.Profesion.Add(profesion);
                    }
                }

                response.Success = true;

                return response;



            }
            catch (Exception ex)
            {
                response.Success = false;
                return response;
                throw;
            }
        }
    }
}
