namespace HDUA.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasenia { get; set; }
        public Boolean Recolector { get; set; }
        public Boolean Estado { get; set; }
        public string Rol { get; set; }
        public string Genero { get; set; }
        public string Tipo { get; set; }
        public string Institucion { get; set; }
    }
}
