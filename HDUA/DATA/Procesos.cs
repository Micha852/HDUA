using MySql.Data.MySqlClient;
//using Org.BouncyCastle.Crypto.Tls;
using HDUA.Models;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;

namespace HDUA.DATA{
    public class Procesos : ConexionMySQL{

        public List<String> Listar(String x){
            var lista = new List<String>();
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand(x, cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()){
                    lista.Add(dr[0] + "");
                }
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
            return lista;
        }

        public List<string> ObtenerMunicipiosPorDepartamento(string nombreDepartamento)
        {
            var listaMunicipios = new List<string>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("LISTARMUNICIPIO", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMBRE", nombreDepartamento);

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    listaMunicipios.Add(dr["NOMBRE_MUNICIPIO"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                desconectar();
            }
            return listaMunicipios;
        }



        public void RegistrarUsuario(UsuarioModel usuario)
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("REGISTRARUSER", cn);
                cmd.Parameters.AddWithValue("NOMBRE", usuario.Nombre);
                cmd.Parameters.AddWithValue("CORREO", usuario.Correo);
                cmd.Parameters.AddWithValue("CONTRA", usuario.Contrasenia);
                cmd.Parameters.AddWithValue("NGENEROUSUARIO", usuario.Genero);
                cmd.Parameters.AddWithValue("NTIPOUSUARIO", usuario.Tipo);
                cmd.Parameters.AddWithValue("NINSTITUCION", usuario.Institucion);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                desconectar();
            }
        }

        public Boolean BUSCARCOREO(String correo, String contra)
        {
            conectar();
            Boolean aux1 = false;
            int aux = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("BUSCARCORREO", cn);
                cmd.Parameters.AddWithValue("CORREO", correo);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader rd = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rd);
                aux = dt.Rows.Count;

                if (aux > 0)
                {
                    cmd = new MySqlCommand("NEWPWORD", cn);
                    cmd.Parameters.AddWithValue("NUEVA", contra);
                    cmd.Parameters.AddWithValue("CORREO", correo);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    aux1 = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                desconectar();
            }

            return aux1;
        }

    }
}
