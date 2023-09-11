using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Request
{
    public class RequestGuardarHorarioBloqueado
    {
        public string fechaInicio { get; set; }
        public string? fechaFin { get; set; }
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
    }
}
