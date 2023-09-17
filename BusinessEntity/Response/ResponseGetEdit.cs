using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Response
{
    public class ResponseGetEdit
    {
        public bool Success { get; set; }
        public ProfesionalViewModel Profesional { get; set; }
        public List<HorarioViewModel> Horario { get; set; }
        public List<ProfesionViewModel> Profesion { get; set; }
        public UsuarioProfesionalViewModel UsuarioProfesional { get; set; }

    }

    public class UsuarioProfesionalViewModel
    {
        public int UsuarioProfesional_Id { get; set; }
        public string User_Id { get; set; }
        public int Profesional_Id { get; set; }
    }
    public class ProfesionViewModel
    {
        public int Profesion_Id { get; set; }

        public string? Nombre { get; set; }

        public string? Rubro { get; set; }
    }
    public class ProfesionalViewModel
    {
        public int Profesional_Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Alias { get; set; }
        public int Profesion_Id { get; set; }
        public int Usuario_Id { get; set; }
        public bool? Activo { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Imagen { get; set; }
        public int? Intervalo { get; set; }
    }

    public class HorarioViewModel
    {
        public int Horario_Id { get; set; }
        public int Profesional_Id { get; set; }
        public TimeSpan Hora { get; set; }
    }
}
