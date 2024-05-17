using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

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
    }
}
