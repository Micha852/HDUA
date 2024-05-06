using HDUA.Models;
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
    }
}
