using DataAccess.Models;
using DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace BusinessEntity.Services
{
    public class MailService
    {
        private DbWrapper _dbWrapper;
        private string _smtpServer = "relay.mailbaby.net";
        private int _smtpPort = 2525;
        private string _smtpUsername = "mb46503";
        private string _smtpPassword = "kzhYsRFyuXt2qrfEBkHS";

        public MailService(DbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;

        }


        public async Task EnviarMailCancelacionTurno(int id)
        {

            try
            {
                var DatosTurno = await _dbWrapper.GetDatosTurno(id);
                if (DatosTurno != null)
                {
                    var Profesional = await _dbWrapper.GetProfesionalById(DatosTurno.Profesional_Id);
                    string Body = $@"<!DOCTYPE html>
                <html>
                <head>
                    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"">
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                        }}
                        .container {{
                            width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #ccc;
                            border-radius: 5px;
                            background-color: #37517e;
                        }}
                        .header {{
                            text-align: center;
                            background-color: #37517e;
                            color: white;
                            padding: 10px;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .sidebar-brand-text {{
                            font-family: Jost;
                            font-weight: 400;
                            color: white;
                            font-size: 24px;
                            text-align: center;
                            background-color: #37517e;
                            margin-bottom: 30px;
                        }}
                    </style>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
                    <div style=""width: 600px; margin: 0 auto; background-color: white; font-weight: 500; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);"">
                                <div style=""background-color: #37517e; text-align: center; padding: 15px; border-radius: 5px; margin-bottom: 30px;"">
            <img src=""https://i.ibb.co/dQYtnwL/logodef.png"" alt=""logodef"" border=""0"" style=""display: block; margin: 0 auto; width: 120px; height: auto;"">
        </div>
                        <div style=""text-align: center;"">
                            <h3 style=""color: #37517e;"">¡Turno Cancelado!</h2>
                        </div>
                        <div style=""margin-top: 20px;"">
                            <p>Hola,</p>
                            <p>Lamentamos informarte que tu turno ha sido cancelado.</p>
                            <p style=""margin-bottom: 25px;"">A continuación, los detalles del turno cancelado:</p>
                            <ul style=""list-style-type: none; padding-left: 0; margin-bottom: 25px;"">
                                <li><strong>Fecha y Hora:</strong> {DatosTurno.FechaHora}</li>
                                <li><strong>Profesional:</strong> {Profesional.Nombre} {Profesional.Apellido}</li>
                            </ul>
                        </div>
                        <div style=""margin-top: 50px;"">
                            <p>Lamentamos cualquier inconveniente que esto pueda causarte.</p>
                            <p>Saludos,</p>
                        </div>
                    </div>
                </body>
                </html>";

                    var ics = $@"BEGIN:VCALENDAR
                             VERSION:2.0
                             PRODID:-//ical.marudot.com//iCal Event Maker
                             CALSCALE:GREGORIAN
                             BEGIN:VEVENT
                             DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
                             UID:{Guid.NewGuid()}@example.com
                             DTSTART:{DatosTurno.FechaHora:yyyyMMddTHHmmssZ}
                             DTEND:{DatosTurno.FechaHora.AddHours(1):yyyyMMddTHHmmssZ}
                             SUMMARY:CANCELADO - Turno con {Profesional.Nombre} {Profesional.Apellido}
                             DESCRIPTION:Este turno ha sido cancelado. Contacte con nosotros para más detalles.
                             STATUS:CANCELLED
                             END:VEVENT
                             END:VCALENDAR";

                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = true;


                        var message = new MailMessage
                        {
                            From = new MailAddress("info@agendario.ar"),
                            Subject = "Turno cancelado - Agendario",
                            Body = Body,
                            IsBodyHtml = true
                        };
                        message.To.Add(DatosTurno.Email);

                        var icsContent = ics;
                        var icsBytes = Encoding.UTF8.GetBytes(icsContent);
                        var memoryStream = new MemoryStream(icsBytes);

                        // Adjuntar el archivo .ics al correo
                        var attachment = new Attachment(memoryStream, "event.ics", "text/calendar");
                        message.Attachments.Add(attachment);
                        await client.SendMailAsync(message);
                        await _dbWrapper.GuardarEvento("Email", $"Mail de cancelacion de turno {id} enviado a{DatosTurno.Email}", "");

                    }
                }
            }
            catch (Exception ex)
            {
                await _dbWrapper.GuardarEvento("Email", ex.Message, "");
            }




        }
    }


}
