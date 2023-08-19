using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class DashboardResponse
    {
        public bool success { get; set; }
        public int cantTurnosTotales { get; set; }
        public int cantTurnosActivos { get; set; }
        public int cantPacientes { get; set; }

        public List<TurnoViewModel> TurnoList { get; set; }

    }

    public class TurnoViewModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? FechaInicio { get; set; }
        public string? FechaFin { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}
