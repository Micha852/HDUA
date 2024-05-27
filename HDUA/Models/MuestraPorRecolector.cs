namespace HDUA.Models
{
    public class MuestraPorRecolector
    {
        public string nombre { get; set; }
        public List<(string, string)>? datos { get; set; }
    }
}
