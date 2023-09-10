using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetHorariosBloqueados
    {
        public bool Success { get; set; }
        public List<DiasBloqueados> DiasBloqueados { get; set; }
    }


    public class DiasBloqueados
    {
        public int HorarioBloqueadoId { get; set; }
        public int CantDias { get; set; }

        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }

    }
}
