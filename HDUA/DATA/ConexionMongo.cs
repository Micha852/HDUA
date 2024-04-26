using MongoDB.Driver;

namespace HDUA.DATA{
    public class ConexionMongo{

        private static ConexionMongo _instance;
        private IMongoDatabase cnm;

        private ConexionMongo(){
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

        private void conectar(){
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
    }
}
