//using BusinessEntity.Models.Request;
//using BusinessEntity.Models.Response;
//using BusinessEntity.Request;
//using BusinessEntity.Response;
//using BusinessEntity.ViewModels;
using BusinessEntity.Models.Request;
using BusinessEntity.Models.Response;
using BusinessEntity.Request;
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

        public async Task<ProfesionalResponse> GetProfesional(string user)
        {
            ProfesionalResponse response = new ProfesionalResponse();

            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    throw new Exception("El profesional no ha sido encontrado");
                }

                response.Profesional_Id = Profesional.Profesional_Id;
                response.Titulo = Profesional.Titulo;

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                await _dbWrapper.GuardarEvento("Ics", $"Error al crear Ics: {ex}", "");
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
            icsContentBuilder.AppendLine($"PRODID:Agendario - {Profesional.Nombre} {Profesional.Apellido}");
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

        public async Task<bool> GuardarHorarioBloqueado(string user, RequestGuardarHorarioBloqueado request)
        {
            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    return false;

                }

                if (request == null) return false;

                DateTime fechaInicio = DateTime.ParseExact($"{request.fechaInicio} {request.horaInicio}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                DateTime fechaFin = request.fechaFin != null
                    ? DateTime.ParseExact($"{request.fechaFin} {request.horaFin}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                    : DateTime.ParseExact($"{request.fechaInicio} {request.horaFin}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);



                AgendaBloqueada HorarioBloqueado = new AgendaBloqueada()
                {
                    Activo = true,
                    FechaDesde = fechaInicio,
                    FechaHasta = fechaFin,
                    Profesional_Id = Profesional.Profesional_Id
                };



                var response = await _dbWrapper.GuardarHorarioBloqueado(HorarioBloqueado);

                return response;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> CancelarHorarioBloqueado(string user, int horarioId)
        {
            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    return false;

                }
                var response = await _dbWrapper.CancelarHorarioBloqueado(Profesional, horarioId);

                return response;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }


        }

        public async Task<ResponseGetHorariosBloqueados> GetHorariosBloqueados(string user)
        {
            ResponseGetHorariosBloqueados response = new ResponseGetHorariosBloqueados();

            try
            {
                var Profesional = await _dbWrapper.ValidateUser(user);

                if (Profesional is null || Profesional.Activo == false)
                {
                    response.Success = false;
                    return response;
                }
                var Turnos = await _dbWrapper.GetHorariosBloqueados(Profesional);


                if (Turnos.Count > 0)
                {
                    var DiasBloqueadosList = new List<DiasBloqueados>();
                    foreach (var item in Turnos)
                    {
                        int diasDiferencia = 1;
                        if (item.FechaHasta.Date > item.FechaDesde.Date)
                        {
                            TimeSpan diferencia = item.FechaHasta - item.FechaDesde;
                            diasDiferencia = diferencia.Days + 1;
                        }


                        var Dia = new DiasBloqueados()
                        {
                            CantDias = diasDiferencia,
                            HorarioBloqueadoId = item.AgendaBloqueada_Id,
                            FechaFin = item.FechaHasta.ToString("dd/MM/yyyy"),
                            FechaInicio = item.FechaDesde.ToString("dd/MM/yyyy"),
                            HoraInicio = item.FechaDesde.ToString("HH:mm"),
                            HoraFin = item.FechaHasta.ToString("HH:mm")
                        };

                        DiasBloqueadosList.Add(Dia);
                    }

                    response.DiasBloqueados = DiasBloqueadosList;
                }
                response.Success = true;

                return response;


            }
            catch (Exception ex)
            {
                response.Success = false;
                throw;
            }
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
                        FechaHora = turno.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                        FechaHoraO = turno.FechaHora.ToString("o"),
                        FechaFinO = turno.FechaHora.AddMinutes(Convert.ToInt32(Profesional.Intervalo)).ToString("o"),
                        Titulo = $"{turno.Nombre} {turno.Apellido}"
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
            bool Success = false;

            try
            {

                Success = await _dbWrapper.CancelarTurnoById(id);
                await _dbWrapper.GuardarEvento("Email", $"Turno {id} cancelado correctamente", "");

                if (Success)
                {
                    await _mailService.EnviarMailCancelacionTurno(id);

                }


            }
            catch (Exception ex)
            {
                await _dbWrapper.GuardarEvento("Email", ex.Message, "");

            }
            return Success;

        }






        public async Task<ResponseGetDiasBloqueados> GetDiasBloqueados(RequestGetDiasBloqueados request)
        {
            ResponseGetDiasBloqueados response = new ResponseGetDiasBloqueados();

            try
            {

                bool DiaActualDisponible = await _validationService.ValidarDiaActual(request.Profesional_Id);

                var agendaBloqueada = _dbWrapper.GetAgendaBloqueada(request.Profesional_Id).Result;

                if (agendaBloqueada != null)
                {
                    response.DiasBloqueados = new List<string>();

                    foreach (var rangoBloqueado in agendaBloqueada)
                    {
                        if (rangoBloqueado.FechaHasta.Date > rangoBloqueado.FechaDesde.Date)
                        {
                            bool InicioCompleto = await _validationService.ValidarDiaCompletoBloqueado(request.Profesional_Id, rangoBloqueado.FechaDesde, true);
                            bool FinCompleto = await _validationService.ValidarDiaCompletoBloqueado(request.Profesional_Id, rangoBloqueado.FechaHasta, false);

                            var DiasBloqueados = await _validationService.ConvertToArrayFechas(rangoBloqueado.FechaDesde, rangoBloqueado.FechaHasta);

                            if (!InicioCompleto)
                            {
                                response.DiasBloqueados.RemoveAll(fecha => fecha == rangoBloqueado.FechaDesde.ToString("yyyy/MM/dd"));
                            }

                            if (!FinCompleto)
                            {
                                response.DiasBloqueados.RemoveAll(fecha => fecha == rangoBloqueado.FechaHasta.ToString("yyyy/MM/dd"));
                            }

                            response.DiasBloqueados.AddRange(DiasBloqueados);
                        }
                        else
                        {
                            bool DiaCompletoBloqueado = await _validationService.ValidarDiaCompletoBloqueado(request.Profesional_Id, rangoBloqueado.FechaHasta, false);

                            var DiasBloqueados = await _validationService.ConvertToArrayFechas(rangoBloqueado.FechaDesde, rangoBloqueado.FechaHasta);
                            response.DiasBloqueados.AddRange(DiasBloqueados);

                            if (!DiaCompletoBloqueado)
                            {
                                response.DiasBloqueados.RemoveAll(fecha => fecha == rangoBloqueado.FechaDesde.ToString("yyyy/MM/dd"));

                            }
                        }
                    }
                }

                //Se obtienen los días que tienen todos los turnos completos y se agregan a la lista de días bloqueados
                response.DiasBloqueados.AddRange(await _validationService.GetDiasTurnosCompletos(request.Profesional_Id));

                if (!DiaActualDisponible)
                {
                    response.DiasBloqueados.Add(fechaActual.ToString("yyyy/MM/dd"));
                }
                //Ordeno lista de días bloqueados
                response.DiasBloqueados.Sort();
                response.Success = true;

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.DiasBloqueados = null;

                return response;
            }
        }


        public async Task<ResponseGetHorasDisponibles> GetHorasDisponibles(RequestGetHorasDisponibles request)
        {
            ResponseGetHorasDisponibles response = new ResponseGetHorasDisponibles();

            try
            {
                var GetHorariosDisponibles = await _validationService.GetHorasDisponibles(request);

                response.Success = true;
                response.HorasDisponibles = GetHorariosDisponibles;
                response.HorasDisponibles.Sort();

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.HorasDisponibles = null;

                return response;
            }
        }

        public async Task<ResponseDatosTurno> GuardarReserva(RequestDatosTurno datosTurno)
        {
            try
            {
                ResponseDatosTurno response = new ResponseDatosTurno();

                if (await _validationService.validateReserva(datosTurno))
                {
                    if (!await _validationService.validateUsuarioTieneTurno(datosTurno))
                    {
                        response.Success = false;
                        response.Estado = "E";
                        response.Mensaje = "Tu mail ya tiene un turno existente en la semana seleccionada";

                    }
                    else
                    {
                        DateTime FechaHora = DateTime.ParseExact($"{datosTurno.Fecha} {datosTurno.Hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                        Turno turno = new Turno()
                        {
                            Nombre = datosTurno.Nombre,
                            Apellido = datosTurno.Apellido,
                            Email = datosTurno.Email,
                            FechaHora = FechaHora,
                            Profesional_Id = datosTurno.ProfesionalId,
                            Telefono = datosTurno.Telefono,
                            Activo = true,
                            Token = _tokenService.GenerateGuidToken()

                        };
                        var TurnoGeneradoDB = await _dbWrapper.AddTurno(turno);
                        await _dbWrapper.GuardarEvento("Turno", $"Turno {TurnoGeneradoDB} creado por profesional {datosTurno.ProfesionalId} guardado correctamente", "");

                        response.Success = true;
                        response.Reserva_Id = TurnoGeneradoDB.Turno_Id;
                        response.Estado = "C";
                        response.TurnoConfirmado = new ResponseTurnoConfirmado()
                        {
                            Cliente = $"{datosTurno.Nombre} {datosTurno.Apellido}",
                            Email = datosTurno.Email,
                            Fecha = datosTurno.Fecha,
                            Hora = datosTurno.Hora,
                            Profesional = datosTurno.Profesional,
                            Telefono = datosTurno.Telefono

                        };
                        //await _mailService.EnviarMailConfirmacionTurno(datosTurno.Email, datosTurno.Profesional, datosTurno.ProfesionalId, TurnoGeneradoDB.FechaHora.ToString("dd/MM/yyyy HH:mm"), TurnoGeneradoDB.FechaHora, TurnoGeneradoDB.Token);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Estado = "E";
                    response.Mensaje = "Ocurrió un error al guardar el turno. Verifique los datos ingresados";
                }
                return response;
            }
            catch (Exception ex)
            {
                await _dbWrapper.GuardarEvento("Turno", $"Error al crear turno manual profesional: {ex}", "");

                ResponseDatosTurno response = new ResponseDatosTurno();

                response.Success = false;
                response.Estado = "E";
                response.Mensaje = $"Ocurrió un error al guardar el turno: {ex.Message}";

                return response;
            }
        }

    }
}
