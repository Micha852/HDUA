using MySql.Data.MySqlClient;
//using Org.BouncyCastle.Crypto.Tls;
using HDUA.Models;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using MongoDB.Driver.Core.Configuration;

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

        public List<string> ObtenerTiposUbicacionPorMunicipio(string nombreMunicipio)
        {
            var listaTiposUbicacion = new List<string>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("LISTARTIPOUBICACION", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMBRE", nombreMunicipio);

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    listaTiposUbicacion.Add(dr["NOMBRE_TIPOUBICACION"].ToString());
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
            return listaTiposUbicacion;
        }

        public List<string> ObtenerUbicacionesPorTipoYMunicipio(string nombreTipo, string nombreMunicipio)
        {
            var listaUbicaciones = new List<string>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("LISTARUBICACION", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NTIPO", nombreTipo);
                cmd.Parameters.AddWithValue("@NMUNICIPIO", nombreMunicipio);

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    listaUbicaciones.Add(dr["NOMBRE_UBICACION"].ToString());
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
            return listaUbicaciones;
        }

        public List<RecolectorModel> ListarRecolector()
        {
            var lista = new List<RecolectorModel>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("RECOLECTORES", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RecolectorModel user = new RecolectorModel()
                    {
                        Nombre = dr[0] + "",
                        Cantidad = Convert.ToInt32(dr[1] + "")
                    };
                    lista.Add(user);
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
            return lista;
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
        public UsuarioModel ValidarUsuario(String NOMBRECORREO, String PWORD)
        {
            conectar();
            DataTable tabla = new DataTable();
            UsuarioModel user = new UsuarioModel();
            try
            {
                MySqlCommand cmd = new MySqlCommand("VALIDARUSUARIO", cn);
                cmd.Parameters.AddWithValue("NOMBRECORREO", NOMBRECORREO);
                cmd.Parameters.AddWithValue("PWORD", PWORD);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    user.Id = Convert.ToInt32(rd[0] + "");
                    user.Nombre = (rd[1] + "");
                    user.Rol = (rd[2] + "");
                }
                tabla.Load(rd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                desconectar();
            }
            return user;
        }
    }
}
