using MySql.Data.MySqlClient;
//using Org.BouncyCastle.Crypto.Tls;
using HDUA.Models;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using MongoDB.Driver.Core.Configuration;
using Org.BouncyCastle.Ocsp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Runtime.Intrinsics.X86;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HDUA.DATA
{
    public class Procesos : ConexionMySQL
    {
        ConexionMongo cnm = new ConexionMongo();

        public List<string> Listar(string x)
        {
            var lista = new List<string>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand(x, cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(dr[0] + "");
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
                        Id = Convert.ToInt32(dr[0] + ""),
                        Nombre = dr[1] + "",
                        Cantidad = Convert.ToInt32(dr[2] + "")
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

        public List<UsuarioModel> ListarUsuarios()
        {
            var lista = new List<UsuarioModel>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("LISTARUSUARIOS", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UsuarioModel user = new UsuarioModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Nombre = dr[1] + "",
                        Recolector = (((dr[2] + "") == "1") ? true : false),
                        Rol = dr[3] + "",
                        Estado = (((dr[4] + "") == "1") ? true : false)
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

        public void EditarUser(int id, string rol, bool rec, bool est)
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("EDITARUSUARIO", cn);
                cmd.Parameters.AddWithValue("USER", id);
                cmd.Parameters.AddWithValue("NROL", rol);
                cmd.Parameters.AddWithValue("REC", rec);
                cmd.Parameters.AddWithValue("EST", est);

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

        public void EditarMiPerfil(int id, string name, string correo, string contra, string generou, string tipou, string instu){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("EDITARMIPERFIL", cn);
                cmd.Parameters.AddWithValue("USER", id);
                cmd.Parameters.AddWithValue("NAME", name);
                cmd.Parameters.AddWithValue("CORREO", correo);
                cmd.Parameters.AddWithValue("CONTRA", contra);
                cmd.Parameters.AddWithValue("GENEROU", generou);
                cmd.Parameters.AddWithValue("TIPOU", tipou);
                cmd.Parameters.AddWithValue("INSTU", instu);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
        }

        public void DesactivarPerfil(int id){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("DesactivarPerfil", cn);
                cmd.Parameters.AddWithValue("id", id);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }
            finally{
                desconectar();
            }
        }

        public void EditarMuestra(int id, string cientifico, string vulgar, string imagen, string coordenada,
            string fecha, string altura, string clase, string orden, string familia, string genero, string especie,
            string ubicacion, string procedencia, string venacion, string forma, string margen, int estado){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("EDITARMUESTRA", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.Parameters.AddWithValue("CIENTIFICO", cientifico);
                cmd.Parameters.AddWithValue("VULGAR", vulgar);
                cmd.Parameters.AddWithValue("IMAGEN", imagen);
                cmd.Parameters.AddWithValue("COORDENADA", coordenada);
                cmd.Parameters.AddWithValue("FECHA", fecha);
                cmd.Parameters.AddWithValue("ALTURA", altura);
                cmd.Parameters.AddWithValue("NCLASE", clase);
                cmd.Parameters.AddWithValue("NORDEN", orden);
                cmd.Parameters.AddWithValue("NFAMILIA", familia);
                cmd.Parameters.AddWithValue("NGENERO", genero);
                cmd.Parameters.AddWithValue("NESPECIE", especie);
                cmd.Parameters.AddWithValue("NUBICACION", ubicacion);
                cmd.Parameters.AddWithValue("NPROCEDENCIA", procedencia);
                cmd.Parameters.AddWithValue("NVENACION", venacion);
                cmd.Parameters.AddWithValue("NFORMA", forma);
                cmd.Parameters.AddWithValue("NMARGEN", margen);
                cmd.Parameters.AddWithValue("ESTADO", estado);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
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

        public void CrearMuestra(MuestraModel muestra)
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("CREARMUESTRA", cn);
                cmd.Parameters.AddWithValue("CIENTIFICO", muestra.Cientifico);
                cmd.Parameters.AddWithValue("VULGAR", muestra.Vulgar);
                cmd.Parameters.AddWithValue("IMAGEN", muestra.Imagen);
                cmd.Parameters.AddWithValue("COORDENADA", muestra.Coordenada);
                cmd.Parameters.AddWithValue("FECHA", muestra.Fecha);
                cmd.Parameters.AddWithValue("NCLASE", muestra.Clase);
                cmd.Parameters.AddWithValue("NORDEN", muestra.Orden);
                cmd.Parameters.AddWithValue("NFAMILIA", muestra.Familia);
                cmd.Parameters.AddWithValue("NGENERO", muestra.Genero);
                cmd.Parameters.AddWithValue("NESPECIE", muestra.Especie);
                cmd.Parameters.AddWithValue("NUBICACION", muestra.Ubicacion);
                cmd.Parameters.AddWithValue("NPROCEDENCIA", muestra.Procedencia);
                cmd.Parameters.AddWithValue("ALTURA", muestra.Altura);
                cmd.Parameters.AddWithValue("NVENACION", muestra.Venacion);
                cmd.Parameters.AddWithValue("NFORMA", muestra.Forma);
                cmd.Parameters.AddWithValue("NMARGEN", muestra.Margen);
                cmd.Parameters.AddWithValue("RECOLECTORES", muestra.Recolectores);
                cmd.Parameters.AddWithValue("IDS", muestra.Ids);

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

        public System.Boolean BUSCARCOREO(System.String correo, System.String contra)
        {
            conectar();
            System.Boolean aux1 = false;
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
        public UsuarioModel ValidarUsuario(System.String NOMBRECORREO, System.String PWORD)
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
                    user.Estado = Convert.ToBoolean(rd[3]) ? true : false;
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

        public List<MuestraModel> ResultadoCientifico(string nombre)
        {
            var lista = new List<MuestraModel>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("BUSCARPORCIENTIFICO", cn);
                cmd.Parameters.AddWithValue("nombre", nombre);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MuestraModel muestra = new MuestraModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Imagen = dr[1] + "",
                        Cientifico = dr[2] + "",
                        Vulgar = dr[3]+"",
                        Coordenada = dr[4] +"",
                        Altura = dr[5] +"",
                        Clase = dr[6] +"",
                        Orden = dr[7] +"",
                        Familia = dr[8] +"",
                        Genero = dr[9] +"",
                        Especie = dr[10] +"",
                        Procedencia = dr[11] +"",
                        Venacion = dr[12] +"",
                        Forma = dr[13] +"",
                        Margen = dr[14] +"",
                        Ubicacion = dr[16] +"",
                        Fecha = dr[17] +"",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false
                    };
                    muestra.Imagen2 = cnm.GetImage(muestra.Imagen);
                    lista.Add(muestra);
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

        public List<MuestraModel> ResultadoVulgar(string nombre)
        {
            var lista = new List<MuestraModel>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("BUSCARPORVULGAR", cn);
                cmd.Parameters.AddWithValue("NOMBRE", nombre);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MuestraModel muestra = new MuestraModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Imagen = dr[1] + "",
                        Vulgar = dr[2] + "",
                        Cientifico = dr[3] + "",
                        Coordenada = dr[4] + "",
                        Altura = dr[5] + "",
                        Clase = dr[6] + "",
                        Orden = dr[7] + "",
                        Familia = dr[8] + "",
                        Genero = dr[9] + "",
                        Especie = dr[10] + "",
                        Procedencia = dr[11] + "",
                        Venacion = dr[12] + "",
                        Forma = dr[13] + "",
                        Margen = dr[14] + "",
                        Ubicacion = dr[16] + "",
                        Fecha = dr[17] + "",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false
                    };
                    muestra.Imagen2 = cnm.GetImage(muestra.Imagen);
                    lista.Add(muestra);
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

        public List<MuestraModel> ResultadoEspecifico(string sorden, string sclase, string sespecie, string sfamilia, string sgenero, string sdepa)
        {
            var lista = new List<MuestraModel>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("BUSCARPARAMETROS", cn);
                cmd.Parameters.AddWithValue("sorden", sorden);
                cmd.Parameters.AddWithValue("sclase", sclase);
                cmd.Parameters.AddWithValue("sespecie", sespecie);
                cmd.Parameters.AddWithValue("sfamilia", sfamilia);
                cmd.Parameters.AddWithValue("sgenero", sgenero);
                cmd.Parameters.AddWithValue("sdepa", sdepa);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MuestraModel muestra = new MuestraModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Imagen = dr[1] + "",
                        Vulgar = dr[2] + "",
                        Cientifico = dr[3] + "",
                        Coordenada = dr[4] + "",
                        Altura = dr[5] + "",
                        Clase = dr[6] + "",
                        Orden = dr[7] + "",
                        Familia = dr[8] + "",
                        Genero = dr[9] + "",
                        Especie = dr[10] + "",
                        Procedencia = dr[11] + "",
                        Venacion = dr[12] + "",
                        Forma = dr[13] + "",
                        Margen = dr[14] + "",
                        Ubicacion = dr[16] + "",
                        Fecha = dr[17] + "",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false
                    };
                    muestra.Imagen2 = cnm.GetImage(muestra.Imagen);
                    lista.Add(muestra);
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

        public MuestraModel FichaMuestra(int id)
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("FICHAMUESTRA", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    MuestraModel muestra = new MuestraModel()
                    {
                        Id = Convert.ToInt32(rd[0]),
                        Cientifico = rd[1] + "",
                        Vulgar = rd[2] + "",
                        Imagen = rd[3] + "",
                        Coordenada = rd[4] + "",
                        Altura = rd[5] + "",
                        Clase = rd[6] + "",
                        Orden = rd[7] + "",
                        Familia = rd[8] + "",
                        Genero = rd[9] + "",
                        Especie = rd[10] + "",
                        Procedencia = rd[11] + "",
                        Venacion = rd[12] + "",
                        Forma = rd[13] + "",
                        Margen = rd[14] + "",
                        Ubicacion = rd[15] + "",
                        ListaRecolectores = rd[16] + "",
                        Fecha = rd[17]+""

                    };
                    muestra.Imagen2 = cnm.GetImage(muestra.Imagen);
                    return muestra;
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
            return null;
        }

        public UsuarioModel DatosMiPerfil(int id)
        {
            conectar();
            UsuarioModel user = new UsuarioModel();
            try
            {
                MySqlCommand cmd = new MySqlCommand("DATOSMIPERFIL", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    user.Id = Convert.ToInt32(rd[0]);
                    user.Nombre = rd[1] + "";
                    user.Correo = rd[2] + "";
                    user.Contrasenia = rd[3] + "";
                    user.Genero = rd[4] + "";
                    user.Institucion = rd[5] + "";
                    user.Tipo = rd[6] + "";
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
            return user;
        }

        public void CrearUbicacion(UbicacionModel ubi){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("INSERTARUBICACION", cn);
                cmd.Parameters.AddWithValue("MUNI", ubi.Municipio);
                cmd.Parameters.AddWithValue("TIPOUBI", ubi.Tipoubi);
                cmd.Parameters.AddWithValue("NOMBRE", ubi.Nombre);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
        }

        public void CrearParametros(string parametro, string nombre){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("INSERTARPARAMETROS", cn);
                cmd.Parameters.AddWithValue("PARAMETRO", parametro);
                cmd.Parameters.AddWithValue("NOMBRE", nombre);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
        }

        public List<ComentarioModel> CrearComentario(ComentarioModel com){
            conectar();
            var lista = new List<ComentarioModel>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("CREARCOMENTARIO", cn);
                cmd.Parameters.AddWithValue("TEXTO", com.Texto);
                cmd.Parameters.AddWithValue("IDMUESTRA", com.Muestra);
                cmd.Parameters.AddWithValue("IDUSUARIO", com.Usuario);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ComentarioModel comentario = new ComentarioModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Usuario = dr[1] + "",
                        Texto = dr[2] + ""
                    };
                    lista.Add(comentario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
            return lista;
        }

        public List<ComentarioModel> ComentariosMuestra(int id){
            var lista = new List<ComentarioModel>();
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("LISTARCOMENTARIOS", cn);
                cmd.Parameters.AddWithValue("IDMUESTRA", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()){
                    ComentarioModel comentario = new ComentarioModel()
                    {
                        Id = Convert.ToInt32(dr[0] + ""),
                        Usuario = dr[1]+"",
                        Texto = dr[2]+""
                    };
                    lista.Add(comentario);
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
            return lista;
        }

        public List<MuestraPorRecolector> InformeMuestrasRecolector1(){
            var informe = new List<MuestraPorRecolector>();
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("RECOLECTORES", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MuestraPorRecolector aux = new MuestraPorRecolector()
                    {
                        nombre = dr[1] + "",
                        datos = InformeMuestrasRecolector2(dr[0] + "")
                    };
                    informe.Add(aux);
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
            return informe;
        }

        public List<(string, string)> InformeMuestrasRecolector2(string id){
            List<(string, string)> datos = new List<(string, string)> ();
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("MUESTRASRECOLECTOR", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    datos.Add((dr[0]+"", dr[1]+""));
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
            return datos;
        }


    }
}
