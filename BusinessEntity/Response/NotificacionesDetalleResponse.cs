using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class NotificacionesDetalleResponse
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string FechaHoraEvento { get; set; }
        public bool Leido { get; set; }
    }
}


