using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class GetItemsPerfilReducidoResponse
    {
        public bool Success { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Activo { get; set; }
        public string Intervalo { get; set; }
        public string Profesion { get; set; }
        public string ActivoDesde { get; set; }
        public string PerfilPublico { get; set; }
        public byte[] ImagenPerfil { get; set; }
    }
}
