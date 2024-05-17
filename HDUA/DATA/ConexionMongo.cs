using HDUA.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HDUA.DATA{
    public class ConexionMongo{

        public static ConexionMongo _instance;
        public IMongoDatabase cnm;

        public ConexionMongo(){
            conectar();
        }

        public static ConexionMongo Instance{
            get{
                if (_instance == null){
                    _instance = new ConexionMongo();
                }
                return _instance;
            }
        }

        public IMongoDatabase Database{
            get { return cnm; }
        }

        public void conectar(){
            try{
                var client = new MongoClient("mongodb://localhost:27017");
                cnm = client.GetDatabase("HDUA");
            }catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        public void desconectar(){
            cnm = null;
        }

        public string UploadImage(ImagenModel img)
        {
            var imageCollection = cnm.GetCollection<ImagenModel>("Imagen");

            imageCollection.InsertOne(img);
            return img.Id;
        }

        public ImagenModel GetImage(string id)
        {
            var imageCollection = cnm.GetCollection<ImagenModel>("Imagen");
            return imageCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefault();
        }

        public void DeleteImage(string id)
        {
            var imageCollection = cnm.GetCollection<ImagenModel>("Imagen");
            imageCollection.DeleteOne(new BsonDocument("_id", new ObjectId(id)));
        }

        public void UpdateImage(ImagenModel img, string id)
        {
            var imageCollection = cnm.GetCollection<ImagenModel>("Imagen");
            var filtro = Builders<ImagenModel>.Filter.Eq("Id", id);
            var update = Builders<ImagenModel>.Update.Set("Imagen", img.Imagen);
            imageCollection.UpdateOne(filtro, update);
        }

    }
}
