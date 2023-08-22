//using BusinessEntity.Models.Request;
//using BusinessEntity.Models.Response;
//using BusinessEntity.Request;
//using BusinessEntity.Response;
//using BusinessEntity.ViewModels;
using BusinessEntity.Response;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace BusinessEntity.Services
{
    public class ReservaService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;
        private MailService _mailService;
        private readonly DateTime fechaActual = DateTime.Now;


        public ReservaService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService, MailService mailService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;
            _mailService = mailService;
        }

        public async Task<string> GenerarIcs(string user)
        {
            ResponseGenerarIcs response = new ResponseGenerarIcs();
            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    throw new Exception("El profesional no ha sido encontrado");
                }

                var Turnos = await _dbWrapper.ExportTurnosByUser(Profesional);

                var Ics = await IcsBuilder(Turnos, Profesional);

                return Ics;


            }
            catch (Exception ex)
            {
                throw;
                // Manejar excepciones si es necesario
            }

        }

        private async Task<string> IcsBuilder(List<Turno> Turnos, Profesional Profesional)
        {
            StringBuilder icsContentBuilder = new StringBuilder();

            List<string> veventSections = new List<string>();

            foreach (var turno in Turnos)
            {
                StringBuilder veventSectionBuilder = new StringBuilder();
                veventSectionBuilder.AppendLine("BEGIN:VEVENT");

                if (!turno.Activo)
                {
                    veventSectionBuilder.AppendLine("METHOD:CANCEL");
                    veventSectionBuilder.AppendLine("STATUS:CANCELLED");
                }

                veventSectionBuilder.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
                veventSectionBuilder.AppendLine($"UID:{turno.Turno_Id}");
                veventSectionBuilder.AppendLine($"DTSTART;TZID=America/Argentina/Buenos_Aires:{turno.FechaHora.ToString("yyyyMMddTHHmmssZ")}");
                veventSectionBuilder.AppendLine($"DTEND;TZID=America/Argentina/Buenos_Aires:{turno.FechaHora.AddMinutes(Convert.ToInt32(Profesional.Intervalo)).ToString("yyyyMMddTHHmmssZ")}");
                veventSectionBuilder.AppendLine($"SUMMARY:Turno con {turno.Nombre} {turno.Apellido}");
                veventSectionBuilder.AppendLine($"DESCRIPTION:Turno con {turno.Nombre} {turno.Apellido}");
                veventSectionBuilder.AppendLine("END:VEVENT");

                veventSections.Add(veventSectionBuilder.ToString());
            }

            string combinedVEvents = string.Join(Environment.NewLine, veventSections);

            icsContentBuilder.AppendLine("BEGIN:VCALENDAR");
            icsContentBuilder.AppendLine("VERSION:2.0");
            icsContentBuilder.AppendLine($"PRODID:Appoint.ar - {Profesional.Nombre} {Profesional.Apellido}");
            icsContentBuilder.AppendLine("CALSCALE:GREGORIAN");
            icsContentBuilder.AppendLine("BEGIN:VTIMEZONE");
            icsContentBuilder.AppendLine("TZID:America/Argentina/Buenos_Aires");
            icsContentBuilder.AppendLine("X-LIC-LOCATION:America/Argentina/Buenos_Aires");
            icsContentBuilder.AppendLine("BEGIN:DAYLIGHT");
            icsContentBuilder.AppendLine("TZOFFSETFROM:-0300");
            icsContentBuilder.AppendLine("TZOFFSETTO:-0200");
            icsContentBuilder.AppendLine("TZNAME:ART");
            icsContentBuilder.AppendLine("DTSTART:19701011T000000");
            icsContentBuilder.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=10;BYDAY=2SU");
            icsContentBuilder.AppendLine("END:DAYLIGHT");
            icsContentBuilder.AppendLine("BEGIN:STANDARD");
            icsContentBuilder.AppendLine("TZOFFSETFROM:-0200");
            icsContentBuilder.AppendLine("TZOFFSETTO:-0300");
            icsContentBuilder.AppendLine("TZNAME:ART");
            icsContentBuilder.AppendLine("DTSTART:19700315T000000");
            icsContentBuilder.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=3SU");
            icsContentBuilder.AppendLine("END:STANDARD");
            icsContentBuilder.AppendLine("END:VTIMEZONE");
            icsContentBuilder.AppendLine(combinedVEvents);
            icsContentBuilder.AppendLine("END:VCALENDAR");

            string icsContent = icsContentBuilder.ToString();

            return icsContent;




        }


        public async Task<ResponseGetTurnosConfirmados> GetTurnosConfirmados(string user)
        {
            ResponseGetTurnosConfirmados response = new ResponseGetTurnosConfirmados();

            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    response.Success = false;
                    return response;
                }

                var Turnos = await _dbWrapper.GetTurnosVigentesByUser(Profesional);
                response.Success = true;
                List<TurnoConfirmado> list = new List<TurnoConfirmado>();

                foreach (var turno in Turnos)
                {
                    TurnoConfirmado t = new TurnoConfirmado()
                    {
                        Turno_Id = turno.Turno_Id,
                        Nombre = turno.Nombre,
                        Apellido = turno.Apellido,
                        Telefono = turno.Telefono,
                        Email = turno.Email,
                        FechaHora = turno.FechaHora.ToString("dd/MM/yyyy HH:mm")
                    };

                    list.Add(t);
                }
                list = list.OrderBy(t => DateTime.ParseExact(t.FechaHora, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                           .ToList();

                response.Turnos = list;
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<bool> CancelarTurno(int id)
        {
            bool Success;

            Success = await _dbWrapper.CancelarTurnoById(id);

            return Success;
        }






        //public async Task<ResponseGetDiasBloqueados> GetDiasBloqueados(RequestGetDiasBloqueados request)
        //{
        //    ResponseGetDiasBloqueados response = new ResponseGetDiasBloqueados();

        //    try
        //    {

        //        bool DiaActualDisponible = await _validationService.ValidarDiaActual(request.Profesional_Id);

        //        var agendaBloqueada = _dbWrapper.GetAgendaBloqueada(request.Profesional_Id).Result;

        //        if (agendaBloqueada != null)
        //        {
        //            response.DiasBloqueados = new List<string>();
        //            foreach (var rangoBloqueado in agendaBloqueada)
        //            {
        //                var DiasBloqueados = await _validationService.ConvertToArrayFechas(rangoBloqueado.FechaDesde, rangoBloqueado.FechaHasta);
        //                response.DiasBloqueados.AddRange(DiasBloqueados);

        //            }
        //        }

        //        //Se obtienen los días que tienen todos los turnos completos y se agregan a la lista de días bloqueados
        //        response.DiasBloqueados.AddRange(await _validationService.GetDiasTurnosCompletos(request.Profesional_Id));

        //        if (!DiaActualDisponible)
        //        {
        //            response.DiasBloqueados.Add(fechaActual.ToString("yyyy/MM/dd"));
        //        }
        //        //Ordeno lista de días bloqueados
        //        response.DiasBloqueados.Sort();
        //        response.Success = true;

        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.DiasBloqueados = null;

        //        return response;
        //    }
        //}


        //public async Task<ResponseGetHorasDisponibles> GetHorasDisponibles(RequestGetHorasDisponibles request)
        //{
        //    ResponseGetHorasDisponibles response = new ResponseGetHorasDisponibles();

        //    try
        //    {
        //        var GetHorariosDisponibles = await _validationService.GetHorasDisponibles(request);

        //        response.Success = true;
        //        response.HorasDisponibles = GetHorariosDisponibles;
        //        response.HorasDisponibles.Sort();

        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.HorasDisponibles = null;

        //        return response;
        //    }
        //}

        //public async Task<ResponseDatosTurno> GuardarReserva(RequestDatosTurno datosTurno)
        //{
        //    try
        //    {
        //        ResponseDatosTurno response = new ResponseDatosTurno();

        //        if (await _validationService.validateReserva(datosTurno))
        //        {
        //            if (!await _validationService.validateUsuarioTieneTurno(datosTurno))
        //            {
        //                response.Success = false;
        //                response.Estado = "E";
        //                response.Mensaje = "Tu mail ya tiene un turno existente en la semana seleccionada";

        //            }
        //            else
        //            {
        //                DateTime FechaHora = DateTime.ParseExact($"{datosTurno.Fecha} {datosTurno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

        //                Turno turno = new Turno()
        //                {
        //                    Nombre = datosTurno.Nombre,
        //                    Apellido = datosTurno.Apellido,
        //                    Email = datosTurno.Email,
        //                    FechaHora = FechaHora,
        //                    Profesional_Id = datosTurno.ProfesionalId,
        //                    Telefono = datosTurno.Telefono,
        //                    Activo = true,
        //                    Token = _tokenService.GenerateGuidToken()

        //                };
        //                var TurnoGeneradoDB = await _dbWrapper.AddTurno(turno);

        //                response.Success = true;
        //                response.Reserva_Id = TurnoGeneradoDB.Turno_Id;
        //                response.Estado = "C";
        //                response.TurnoConfirmado = new ResponseTurnoConfirmado()
        //                {
        //                    Cliente = $"{datosTurno.Nombre} {datosTurno.Apellido}",
        //                    Email = datosTurno.Email,
        //                    Fecha = datosTurno.Fecha,
        //                    Hora = datosTurno.Hora,
        //                    Profesional = datosTurno.Profesional,
        //                    Telefono = datosTurno.Telefono

        //                };
        //                await _mailService.EnviarMailConfirmacionTurno(datosTurno.Email, datosTurno.Profesional, datosTurno.ProfesionalId, TurnoGeneradoDB.FechaHora.ToString("dd/MM/yyyy HH:mm"), TurnoGeneradoDB.FechaHora, TurnoGeneradoDB.Token);
        //            }
        //        }
        //        else
        //        {
        //            response.Success = false;
        //            response.Estado = "E";
        //            response.Mensaje = "Ocurrió un error al guardar el turno. Verifique los datos ingresados";
        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseDatosTurno response = new ResponseDatosTurno();

        //        response.Success = false;
        //        response.Estado = "E";
        //        response.Mensaje = $"Ocurrió un error al guardar el turno: {ex.Message}";

        //        return response;
        //    }
        //}

        //public async Task<CancelarTurnoViewModel> GetCancelacionTurno(string Token, ProfesionalResponse Profesional)
        //{
        //    CancelarTurnoViewModel response = new CancelarTurnoViewModel();
        //    var Turno = await _dbWrapper.CheckToken(Token);

        //    if (Turno != null)
        //    {
        //        response.Profesional = Profesional.Profesional;
        //        response.Paciente = $"{Turno.Nombre} {Turno.Apellido}";
        //        response.FechaHora = Turno.FechaHora.ToString("dd/MM/yyyy HH:mm");
        //        response.Profesional_Id = Turno.Profesional_Id;
        //        response.Token = Turno.Token;
        //        response.Success = true;

        //    }
        //    else
        //    {
        //        response.Success = false;
        //        response.Mensaje = "No hemos podido encontrar el turno que deseas cancelar";
        //    }

        //    return response;
        //}

        //public async Task<ResponseCancelarTurno> CancelarReserva(RequestCancelarTurno request)
        //{
        //    ResponseCancelarTurno response = new ResponseCancelarTurno();

        //    try
        //    {
        //        ResponseCancelarTurno Validacion = await _validationService.ValidateCancelarReserva(request);


        //        if (Validacion.Success)
        //        {
        //            await _dbWrapper.CancelarTurno(request.Token);

        //            response.Success = true;
        //            response.Mensaje = $"El turno ha sido cancelado correctamente";
        //        }
        //        else
        //        {
        //            response.Success = Validacion.Success;
        //            response.Mensaje = Validacion.Mensaje;

        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {

        //        response.Success = true;
        //        response.Mensaje = $"Hubo un error al cancelar la reserva";
        //        return response;
        //    }
        //}
    }
}
