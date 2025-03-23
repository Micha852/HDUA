using MySql.Data.MySqlClient;
using System.ComponentModel;

namespace HDUA.DATA{
    public class ConexionMySQL{
        protected MySqlConnection cn;

        protected void conectar(){
            try{
                cn = new MySqlConnection("Server=127.0.0.1;Port=3306;Database=hdua;Uid=root;Pwd=root");
                cn.Open();
            }catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
        }
        protected void desconectar() {
            try {
                cn.Close();
            } catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
        }
    }
}