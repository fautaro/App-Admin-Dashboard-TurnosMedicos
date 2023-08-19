using DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using System.Globalization;
using BusinessEntity.Response;

namespace BusinessEntity.Services
{
    public class DashboardService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;
        private MailService _mailService;
        private readonly DateTime fechaActual = DateTime.Now;


        public DashboardService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService, MailService mailService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;
            _mailService = mailService;
        }


        public async Task<DashboardResponse> GetItems(string AuthenticatedUser)
        {
            DashboardResponse dashboardResponse = new DashboardResponse();

            try
            {
                var user = await _dbWrapper.ValidateUser(AuthenticatedUser);

                if (user is null || user.Activo == false)
                {
                    dashboardResponse.success = false;
                    return dashboardResponse;
                }

                var Turnos = await _dbWrapper.GetTurnosByUser(user);
                dashboardResponse.success = true;
                dashboardResponse.cantPacientes = await GetCantPacientes(Turnos);
                dashboardResponse.cantTurnosTotales = await GetTurnosTotales(Turnos);
                dashboardResponse.cantTurnosActivos = await GetTurnosActivos(Turnos);


                dashboardResponse.TurnoList = await MapTurnos(Turnos, user.Intervalo);
                return dashboardResponse;
            }
            catch (Exception ex)
            {
                dashboardResponse.success = false;
                return dashboardResponse;
            }
        }
        private async Task<List<TurnoViewModel>> MapTurnos(List<Turno> turnos, int? intervalo)
        {
            List<TurnoViewModel> turnoList = new List<TurnoViewModel>();

            foreach (var item in turnos)
            {
                TurnoViewModel turno = new TurnoViewModel()
                {
                    Email = item.Email,
                    Nombre = $"{item.Nombre} {item.Apellido}",
                    Telefono = item.Telefono,
                    Titulo = $"Paciente: {item.Nombre} {item.Apellido} - Hora: {item.FechaHora.ToString("dd--MM HH-mm")}",
                    FechaInicio = item.FechaHora.ToString("o"),
                    FechaFin = item.FechaHora.AddMinutes((double)intervalo).ToString("o"),
                    Id = item.Turno_Id
                };
                turnoList.Add(turno);
            }

            return turnoList;
        }

        private async Task<int> GetCantPacientes(List<Turno> turnos)
        {
            int CantidadPacientes = turnos
                .Where(turno => !string.IsNullOrEmpty(turno.Email))
                .Select(turno => turno.Email)
                .Distinct()
                .Count();

            return CantidadPacientes;
        }

        private async Task<int> GetTurnosTotales(List<Turno> turnos)
        {
            int CantidadTurnos = turnos.Count();

            return CantidadTurnos;

        }

        private async Task<int> GetTurnosActivos(List<Turno> turnos)
        {
            int CantidadTurnosActivos = turnos.Where(e => e.Activo == true && e.FechaHora > fechaActual).Count();

            return CantidadTurnosActivos;

        }



    }
}
