using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HDUA.Models
{
    public class ImagenModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }

    }
}
