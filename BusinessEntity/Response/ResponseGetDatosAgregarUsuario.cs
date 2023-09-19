using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetDatosAgregarUsuario
    {
        public bool Success { get; set; }
        public string Usuario_Id { get; set; }
        public List<ProfesionViewModel>? Profesion { get; set; }

    }
}
