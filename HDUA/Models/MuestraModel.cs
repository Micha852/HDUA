namespace HDUA.Models
{
    public class MuestraModel
    {
        public int Id { get; set; }
        public string Cientifico { get; set; }
        public string Vulgar { get; set; }
        public string Imagen { get; set; }
        public string Fecha { get; set; }
        public string Coordenada { get; set; }
        public string Clase { get; set; }
        public string Orden { get; set; }
        public string Familia { get; set; }
        public string Genero { get; set; }
        public string Especie { get; set; }
        public string Ubicacion { get; set; }
        public string Procedencia { get; set; }
        public string Altura { get; set; }
        public string Venacion { get; set; }
        public string Forma { get; set; }
        public string Margen { get; set; }
        public IFormFile? File { get; set; }
        public ImagenModel? Imagen2 { get; set; }
        public List<int> IdsUsuariosSeleccionados { get; set; }
        public int Recolectores { get; set; }
        public string ListaRecolectores { get; set; }
        public string Ids { get; set; }
        public List<ComentarioModel>? comentarios { get; set; }
        public bool Estado { get; set; }
        public string Division { get; set; }
        public string Proyecto { get; set; }
        public string Adultos { get; set; }
        public string Jovenes { get; set; }
        public string Condicion { get; set; }
        public string Origen { get; set; }
        public string Elevacionmin { get; set; }
        public string Elevacionmax { get; set; }
        public string Habitad { get; set; }
        public string ObervacionLocal { get; set; }
        public string Catalogo { get; set; }
        public string Registro { get; set; }
        public string Autor { get; set; }
    }
}