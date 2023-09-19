using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Request
{
    public class RequestGuardarDatosNuevoUsuario
    {
        public string Usuario_Id { get; set; }
        public string profesion_Id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string alias { get; set; }
        public int intervalo { get; set; }
    }

}
