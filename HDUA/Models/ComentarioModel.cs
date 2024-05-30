namespace HDUA.Models
{
    public class ComentarioModel
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string Muestra { get; set; }
        public string Usuario { get; set; }
        public int idUser { get; set; }
    }
}
