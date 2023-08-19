using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{

    public class UsuarioProfesional
    {
        public int UsuarioProfesional_Id { get; set; }
        public string User_Id { get; set; }
        public int Profesional_Id { get; set; }
    }
}
