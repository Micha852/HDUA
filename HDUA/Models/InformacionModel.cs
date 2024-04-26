using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HDUA.Models{
    public class InformacionModel{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Historia { get; set; }
        public string Datos { get; set; }
        public string Contactos { get; set; }
        public string Tyc { get; set; }
    }
}
