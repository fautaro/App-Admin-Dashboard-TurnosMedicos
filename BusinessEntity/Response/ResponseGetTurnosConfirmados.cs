using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetTurnosConfirmados
    {
        public bool Success { get; set; }
        public List<TurnoConfirmado> Turnos { get; set; }



    }

    public class TurnoConfirmado
    {
        public int Turno_Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string FechaHora { get; set; }
        public string FechaHoraO { get; set; }
        public string FechaFinO { get; set; }
        public string Titulo { get; set; }
    }

}
