using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetUsuarios
    {
        public bool Success { get; set; }
        public List<Usuario>? Usuarios { get; set; }
    }

    public class Usuario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int Profesional_Id { get; set; }
        public string Profesion { get; set; }
        public string Activo { get; set; }
    }

}