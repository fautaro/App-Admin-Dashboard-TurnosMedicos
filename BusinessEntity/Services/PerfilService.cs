﻿using BusinessEntity.Request;
using BusinessEntity.Response;
using DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessEntity.Services
{
    public class PerfilService
    {
        private DbWrapper _dbWrapper;
        private ValidationService _validationService;
        private TokenService _tokenService;
        private MailService _mailService;
        private readonly DateTime fechaActual = DateTime.Now;


        public PerfilService(DbWrapper dbWrapper, ValidationService validationService, TokenService tokenService, MailService mailService)
        {
            _dbWrapper = dbWrapper;
            _validationService = validationService;
            _tokenService = tokenService;
            _mailService = mailService;
        }

        public async Task <bool> GuardarPerfilPublico(string id, RequestGuardarPerfilPublico request)
        {
            try
            {
                var result = false; 
                if (request.Imagen == null)
                {
                    result = await _dbWrapper.UpdatePerfilPublico(id, request.Titulo, request.Descripcion, null);

                } else
                {
                    Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                    string convert = regex.Replace(request.Imagen, string.Empty);

                    byte[] image = Convert.FromBase64String(convert);



                    result = await _dbWrapper.UpdatePerfilPublico(id, request.Titulo, request.Descripcion, image);

                }
                return result;
            }
            catch (Exception ex) 
            {
                await _dbWrapper.GuardarEvento("Perfil Publico", $"Error al guardar perfil público: {ex}", "");

                throw;
            }
        }
        public async Task<GetPerfilPublicoResponse> GetItemsPerfilPublico(string id, string email)
        {
            var response = new GetPerfilPublicoResponse();
            try
            {
                var Profesional = await _dbWrapper.ValidateUser(id);

                response.Success = true;
                response.ImagenPerfil = Profesional.Imagen;
                response.Titulo = Profesional.Titulo;
                response.Descripcion = Profesional.Descripcion;

                return response;



            }
            catch (Exception ex)
            {
                response.Success = false;
                throw;
            }

        }

        public async Task<GetItemsPerfilReducidoResponse> GetItemsPerfilReducido(string id, string email)
        {
            var response = new GetItemsPerfilReducidoResponse();

            try
            {
                var user = await _dbWrapper.ValidateUser(id);

                var Profesion = await _dbWrapper.GetProfesion(user);


                response.Success = true;

                response.NombreUsuario = $"{user.Nombre} {user.Apellido}";
                response.ImagenPerfil = user.Imagen;
                response.Email = email;
                response.Intervalo = $"{user.Intervalo} minutos";
                response.Activo = user.Activo == true ? "Sí" : "No";
                response.PerfilPublico = $"https://agendario.ar/{user.Alias}";

                if (Profesion != null)
                {
                    response.Profesion = Profesion.Nombre;

                } else
                {
                    response.Profesion = "No hay datos registrados";
                }

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                return response;
            }
        }


    }
}
