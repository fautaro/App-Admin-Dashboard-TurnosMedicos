using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class GetPerfilPublicoResponse
    {
        public bool Success { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public byte[]? ImagenPerfil { get; set; }
        public byte[]? FondoImagen { get; set; }
        public string? FondoColor { get; set; }



    }
}
