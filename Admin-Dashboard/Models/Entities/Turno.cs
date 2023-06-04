namespace Admin_Dashboard.Models.Entities
{
    public class Turno
    {
        public int Turno_Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; }
        public int Profesional_Id { get; set; }
        public string? Link { get; set; }
    }
}
