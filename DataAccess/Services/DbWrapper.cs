﻿using DataAccess.Context;
using DataAccess.Models;
using DataAccess.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Services
{
    public class DbWrapper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DateTime fechaActual = DateTime.Now;

        public DbWrapper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region Metodos Generales
        public async Task GuardarEvento(string? Entidad, string Detalle, string? Usuario_Id)
        {
            Evento evento = new Evento();
            try
            {
                evento.Entidad = Entidad;
                evento.Detalle = Detalle;
                evento.Usuario_Id = Usuario_Id;
                evento.FechaHora = DateTime.Now;

                _dbContext.Evento.AddAsync(evento);
                _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Administrador> GetAdmin(string user)
        {
            try
            {
                var admin = await _dbContext.Administrador.Where(e => e.Administrador_Id == user).FirstOrDefaultAsync();

                return admin;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Profesion> GetProfesion(Profesional profesional)
        {
            try
            {
                var Profesion = await _dbContext.Profesion.Where(e => e.Profesion_Id == profesional.Profesion_Id).FirstOrDefaultAsync();
                return Profesion;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Profesional> ValidateUser(string User)
        {
            try
            {
                var user = await _dbContext.UsuarioProfesional.Where(e => e.User_Id == User).FirstOrDefaultAsync();

                if (user == null)
                    throw new Exception("Profesional no encontrado");

                var profesional = await _dbContext.Profesional.Where(e => e.Profesional_Id == user.Profesional_Id).FirstOrDefaultAsync();

                return profesional;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<List<Turno>> GetTurnosVigentesByUser(Profesional profesional)
        {
            var Turnos = await _dbContext.Turno.Where(e => e.Activo == true
                                                        && e.Profesional_Id == profesional.Profesional_Id && e.FechaHora > fechaActual).ToListAsync();

            return Turnos;
        }

        public async Task<List<Turno>> ExportTurnosByUser(Profesional profesional)
        {
            var Turnos = await _dbContext.Turno.Where(e => e.Profesional_Id == profesional.Profesional_Id && e.FechaHora > fechaActual).ToListAsync();

            return Turnos;
        }
        #endregion
        #region MailService
        public async Task<Turno> GetDatosTurno (int id)
        {
            try
            {
                var Turno = await _dbContext.Turno.Where(e => e.Turno_Id == id).FirstOrDefaultAsync();

                return Turno;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Recuperar Clave
        //public async Task<string> GetPasswordUser(string email)
        //{
        //    try
        //    {
        //        var clave = await _dbContext.
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        #endregion
        #region Admin Service

        //Guardar Horarios

        public async Task<bool> GuardarHorariosProfesional(List<Horario> horarios)
        {
            try
            {
                if (horarios != null)
                {
                    await _dbContext.AddRangeAsync(horarios);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        //Crear profesional
        public async Task<bool> CrearRelUsuarioProfesional(UsuarioProfesional usuarioProfesional)
        {
            try
            {
                await _dbContext.UsuarioProfesional.AddAsync(usuarioProfesional);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<int> CrearProfesional(Profesional profesional)
        {
            try
            {
                await _dbContext.Profesional.AddAsync(profesional);
                await _dbContext.SaveChangesAsync();
                return profesional.Profesional_Id;
            }
            catch (Exception ex)
            {
                return 0;
                throw;
            }
        }

        //Get UsuarioProfesional

        public async Task<UsuarioProfesional> GetUsuarioProfesionalAdmin(string User)
        {
            try
            {
                var ProfesionesList = await _dbContext.UsuarioProfesional.Where(e => e.User_Id == User).FirstOrDefaultAsync();

                return ProfesionesList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Get Profesiones

        public async Task<List<Profesion>> GetProfesionesAdmin()
        {
            try
            {
                var ProfesionesList = await _dbContext.Profesion.ToListAsync();

                return ProfesionesList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Get Profesional para editar

        public async Task<Profesional> GetProfesionalAdmin(string User, UsuarioProfesional usprof)
        {
            try
            {

                var ProfesionalList = await _dbContext.Profesional.Where(e => e.Profesional_Id == usprof.Profesional_Id).FirstOrDefaultAsync();

                return ProfesionalList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Get listado de horarios definidos
        public async Task<List<Horario>> GetUserHorariosAdmin(string User, UsuarioProfesional usprof)
        {
            try
            {
                var horariosList = await _dbContext.Horario.Where(e => e.Profesional_Id == usprof.Profesional_Id).ToListAsync();

                return horariosList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<AdminDashboardResult>> GetAdminDashboard()
        {
            try
            {
                var adminDashboardResults = await _dbContext.AdminDashboardResults
                    .FromSqlRaw("EXEC AdminDashboard_Get")
                    .ToListAsync();


                return adminDashboardResults;


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Administrador> ValidateAdmin(string User)
        {
            try
            {
                var user = await _dbContext.Administrador.Where(e => e.Administrador_Id == User).FirstOrDefaultAsync();

                return user;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        
        #endregion
        #region Perfil
        public async Task<bool> UpdatePerfilPublico(string id, string? Titulo, string? Descripcion, byte[]? imagen)
        {
            var user = await _dbContext.UsuarioProfesional.Where(e => e.User_Id == id).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Profesional no encontrado");

            var profesional = await _dbContext.Profesional.Where(e => e.Profesional_Id == user.Profesional_Id).FirstOrDefaultAsync();

            if (Titulo != null)
            {
                profesional.Titulo = Titulo;

            }
            if (Descripcion != null)
            {
                profesional.Descripcion = Descripcion;

            }
            if (imagen != null)
            {
                profesional.Imagen = imagen;

            }

            _dbContext.Update(profesional);

            await _dbContext.SaveChangesAsync();

            return true;
        }



        #endregion
        #region Acciones

        //Guardar horario bloqueado

        public async Task<bool> GuardarHorarioBloqueado(AgendaBloqueada request)
        {

            try
            {
                await _dbContext.AgendaBloqueada.AddAsync(request);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        //Obtener horarios bloqueados
        public async Task<List<AgendaBloqueada>> GetHorariosBloqueados(Profesional profesional)
        {
            try
            {

                var HorariosBloqueados = await _dbContext.AgendaBloqueada.Where(e => e.Activo && e.FechaDesde > fechaActual && e.Profesional_Id == profesional.Profesional_Id).ToListAsync();

                return HorariosBloqueados;


            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //Eliminar horario bloqueado

        public async Task<bool> CancelarHorarioBloqueado(Profesional profesional, int horarioId)
        {
            try
            {
                var CancelarHorario = await _dbContext.AgendaBloqueada.Where(e => e.AgendaBloqueada_Id == horarioId && e.Profesional_Id == profesional.Profesional_Id).FirstOrDefaultAsync();

                if (CancelarHorario != null)
                {
                    CancelarHorario.Activo = false;
                    _dbContext.Update(CancelarHorario);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        // Eliminar un turno
        public async Task<bool> CancelarTurnoById(int id)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Turno_Id == id).FirstOrDefaultAsync();

                TurnoACancelar.Activo = false;
                _dbContext.Turno.Update(TurnoACancelar);
                await _dbContext.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                await GuardarEvento("Turno", ex.Message, "");

                return false;
                throw;
            }

        }

        #endregion
        #region Dashboard
        public async Task<bool> BorrarNotificaciones(Profesional profesional)
        {
            try
            {
                var Notificaciones = await _dbContext.Notificacion.Where(e => e.Profesional_Id == profesional.Profesional_Id && (e.Eliminado == null || e.Eliminado == false)).ToListAsync();

                foreach (var item in Notificaciones)
                {
                    item.Eliminado = true;
                    _dbContext.Update(item);


                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> MarcarNotificacionesLeidas(Profesional profesional)
        {
            try
            {
                var Notificaciones = await _dbContext.Notificacion.Where(e => e.Profesional_Id == profesional.Profesional_Id && (e.Eliminado == null || e.Eliminado == false) && e.Leido == false).ToListAsync();

                foreach (var item in Notificaciones)
                {
                    item.Leido = true;
                    _dbContext.Update(item);


                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }
        public async Task<int> GetCantNotificacionesByUser(Profesional profesional)
        {
            var Notificaciones = await _dbContext.Notificacion.Where(e => e.Profesional_Id == profesional.Profesional_Id && (e.Eliminado == null || e.Eliminado == false) && e.Leido == false).ToListAsync();

            return Notificaciones.Count();

        }
        public async Task<List<Notificacion>> GetNotificacionesDetalleByUser(Profesional profesional)
        {
            var Notificaciones = await _dbContext.Notificacion.Where(e => e.Profesional_Id == profesional.Profesional_Id && (e.Eliminado == null || e.Eliminado == false)).OrderBy(e => e.FechaHoraEvento).ToListAsync();
            return Notificaciones;
        }
        public async Task<List<Turno>> GetTurnosByUser(Profesional profesional)
        {
            var Turnos = await _dbContext.Turno.Where(e => e.Activo == true
                                                        && e.Profesional_Id == profesional.Profesional_Id).ToListAsync();

            return Turnos;
        }

        #endregion

        #region Metodos del otro sitio
        // Agregar un nuevo turno
        public async Task<Turno> AddTurno(Turno turno)
        {
            try
            {
                await _dbContext.Turno.AddAsync(turno);
                await _dbContext.SaveChangesAsync();

                return turno;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        // Eliminar un turno
        public async Task<bool> CancelarTurno(string token)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Token == token).FirstOrDefaultAsync();

                TurnoACancelar.Activo = false;
                _dbContext.Turno.Update(TurnoACancelar);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }


        //Obtener Agenda Bloqueada
        public async Task<List<AgendaBloqueada>> GetAgendaBloqueada(int profesional_Id)
        {
            try
            {

                List<AgendaBloqueada> agendaBloqueadaList = await _dbContext.AgendaBloqueada
                    .Where(e => e.Profesional_Id == profesional_Id && e.Activo && e.FechaDesde > fechaActual)
                    .ToListAsync();

                // Devolver la lista obtenida
                return agendaBloqueadaList;

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        // Metodos para Capa de validación

        //Metodo que devuelve true si el usuario ya tiene un turno para la semana seleccionada
        public async Task<bool> GetTurnosSemana(int profesional_Id, string email, DateTime fecha, DayOfWeek diaTurno, DateTime primerDiaSemana, DateTime ultimoDiaSemana)
        {
            try
            {
                // Verificar si existe algún turno en la misma semana y con el mismo correo electrónico
                var turnoExistente = await _dbContext.Turno
                    .AnyAsync(e => e.Profesional_Id == profesional_Id &&
                                   e.Email == email &&
                                   e.FechaHora >= primerDiaSemana && e.FechaHora <= ultimoDiaSemana && e.FechaHora > fechaActual && e.Activo);

                return turnoExistente;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<AgendaBloqueada>> GetHorariosBloqueadosDiaSeleccionado (int profesional_Id, DateTime fecha)
        {
            try
            {
                var AgendaBloqueada = await _dbContext.AgendaBloqueada.Where(e => e.Activo == true && (e.FechaDesde.Date == fecha.Date || e.FechaHasta.Date == fecha.Date)).ToListAsync();

                return AgendaBloqueada;
            }
            catch (Exception ex)
            {

                throw;
            }    
        }
        public async Task<List<Turno>> GetTurnosReservados(int profesional_Id)
        {
            try
            {
                var TurnosReservados = await _dbContext.Turno.Where(e => e.Profesional_Id == profesional_Id && e.Activo && e.FechaHora > fechaActual).ToListAsync();

                return TurnosReservados;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<Turno>> GetTurnosReservadosDelDia(int profesional_Id)
        {
            try
            {
                var TurnosReservados = await _dbContext.Turno.Where(e => e.Profesional_Id == profesional_Id && e.Activo && e.FechaHora == fechaActual).ToListAsync();

                return TurnosReservados;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<Horario>> GetHorariosPermitidos(int profesional_Id)
        {

            try
            {
                var HorariosPermitidos = await _dbContext.Horario.Where(e => e.Profesional_Id == profesional_Id).ToListAsync();

                return HorariosPermitidos;


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Turno> CheckTurno(DateTime fechaHora, int profesionalId)
        {
            try
            {
                var TurnoRepetido = await _dbContext.Turno.Where(e => e.FechaHora == fechaHora && e.Profesional_Id == profesionalId && e.Activo).FirstOrDefaultAsync();

                return TurnoRepetido;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<Turno> CheckToken(string token)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Token == token && e.FechaHora > fechaActual).FirstOrDefaultAsync();

                return TurnoACancelar;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Turno> CheckReservaId(int TurnoId, string token)
        {
            try
            {
                var TurnoACancelar = await _dbContext.Turno.Where(e => e.Turno_Id == TurnoId).FirstOrDefaultAsync();

                return TurnoACancelar;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<Profesional> GetProfesional(string profesional)
        {
            try
            {
                var Profesional = await _dbContext.Profesional.Where(e => e.Alias == profesional).FirstOrDefaultAsync();

                return Profesional;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Profesional> GetProfesionalById(int Profesional_Id)
        {
            try
            {
                var Profesional = await _dbContext.Profesional.Where(e => e.Profesional_Id == Profesional_Id).FirstOrDefaultAsync();

                return Profesional;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
