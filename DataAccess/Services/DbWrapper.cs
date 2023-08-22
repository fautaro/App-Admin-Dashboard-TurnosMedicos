﻿using DataAccess.Context;
using DataAccess.Models;
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

        #region Acciones
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
                return false;
                throw;
            }

        }

        #endregion
        #region Dashboard

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

        #endregion
    }
}
