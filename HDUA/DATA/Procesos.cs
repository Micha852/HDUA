using MySql.Data.MySqlClient;
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
using ClosedXML.Excel;
using HDUA.Helpers;


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

        public void EditarMiPerfil(int id, string name, string correo, string contra, string generou, string tipou, string instu)
        {
            conectar();
            try
            {
                MySqlCommand getPassCmd = new MySqlCommand("SELECT CONTRASENIA_USUARIO FROM usuario WHERE ID_USUARIO = @id", cn);
                getPassCmd.Parameters.AddWithValue("@id", id);
                string currentHashedPassword = getPassCmd.ExecuteScalar()?.ToString();

                string hashedPassword = (!string.IsNullOrEmpty(contra)) ? PasswordHelper.HashPassword(contra) : currentHashedPassword;

                MySqlCommand cmd = new MySqlCommand("EDITARMIPERFIL", cn);
                cmd.Parameters.AddWithValue("USER", id);
                cmd.Parameters.AddWithValue("NAME", name);
                cmd.Parameters.AddWithValue("CORREO", correo);
                cmd.Parameters.AddWithValue("CONTRA", hashedPassword);
                cmd.Parameters.AddWithValue("GENEROU", generou);
                cmd.Parameters.AddWithValue("TIPOU", tipou);
                cmd.Parameters.AddWithValue("INSTU", instu);

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

        public UsuarioModel ValidarUsuarioPorId(int userId)
        {
            conectar();
            UsuarioModel user = null;

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT ID_USUARIO, CONTRASENIA_USUARIO FROM usuario WHERE ID_USUARIO = @id", cn);
                cmd.Parameters.AddWithValue("@id", userId);

                MySqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    user = new UsuarioModel
                    {
                        Id = rd.GetInt32("ID_USUARIO"),
                        Contrasenia = rd["CONTRASENIA_USUARIO"].ToString()
                    };
                }
                rd.Close();
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
            string ubicacion, string procedencia, string venacion, string forma, string margen, int estado,
            int cantidad, string ids, string division, string proyecto, string adultos, string jovenes, string condicion,
            string origen, string elevacionmin, string elevacionmax, string habitad, string observacion,
            string catalogo, string registro, string autor){
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
                cmd.Parameters.AddWithValue("CANTIDAD", cantidad);
                cmd.Parameters.AddWithValue("IDS", ids);
                cmd.Parameters.AddWithValue("NDIVISION", division);
                cmd.Parameters.AddWithValue("NPROYECTO", proyecto);
                cmd.Parameters.AddWithValue("ADULTOS", adultos);
                cmd.Parameters.AddWithValue("JOVENES", jovenes);
                cmd.Parameters.AddWithValue("NCONDICION", condicion);
                cmd.Parameters.AddWithValue("NORIGEN", origen);
                cmd.Parameters.AddWithValue("ELEVACIONMIN", elevacionmin);
                cmd.Parameters.AddWithValue("ELEVACIONMAX", elevacionmax);
                cmd.Parameters.AddWithValue("NHABITAD", habitad);
                cmd.Parameters.AddWithValue("NOBSERVACION", observacion);
                cmd.Parameters.AddWithValue("NCATALOGO", catalogo);
                cmd.Parameters.AddWithValue("NREGISTRO", registro);
                cmd.Parameters.AddWithValue("NAUTOR", autor);
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

                // Encripta la contraseña antes de enviarla
                string hashedPassword = PasswordHelper.HashPassword(usuario.Contrasenia);

                cmd.Parameters.AddWithValue("NOMBRE", usuario.Nombre);
                cmd.Parameters.AddWithValue("CORREO", usuario.Correo);
                cmd.Parameters.AddWithValue("CONTRA", hashedPassword); // Usar la contraseña encriptada
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
                cmd.Parameters.AddWithValue("NDIVISION", muestra.Division);
                cmd.Parameters.AddWithValue("NPROYECTO", muestra.Proyecto);
                cmd.Parameters.AddWithValue("CADULTOS", muestra.Adultos);
                cmd.Parameters.AddWithValue("CJOVENES", muestra.Jovenes);
                cmd.Parameters.AddWithValue("NCONDICION", muestra.Condicion);
                cmd.Parameters.AddWithValue("NORIGEN", muestra.Origen);
                cmd.Parameters.AddWithValue("ELEVACIONMIN", muestra.Elevacionmin);
                cmd.Parameters.AddWithValue("ELEVACIONMAX", muestra.Elevacionmax);
                cmd.Parameters.AddWithValue("NHABITAD", muestra.Habitad);
                cmd.Parameters.AddWithValue("NOBSERVACION", muestra.ObervacionLocal);
                cmd.Parameters.AddWithValue("NCATALOGO", muestra.Catalogo);
                cmd.Parameters.AddWithValue("NREGISTRO", muestra.Registro);
                cmd.Parameters.AddWithValue("NOMBREAUTOR", muestra.Autor);

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

        public void CrearProyecto(ProyectoModel proyecto)
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("CREARPROYECTO", cn);
                cmd.Parameters.AddWithValue("NOMBRE", proyecto.Nombre);
                cmd.Parameters.AddWithValue("INICIO", proyecto.Inicio);
                cmd.Parameters.AddWithValue("FIN", proyecto.Fin);
                cmd.Parameters.AddWithValue("ESFUERZO", proyecto.Esfuerzo);
                cmd.Parameters.AddWithValue("OBSERVACION", proyecto.Observacion);
                cmd.Parameters.AddWithValue("NPROTOCOLO", proyecto.Protocolo);
                cmd.Parameters.AddWithValue("HINICIO", proyecto.Hinicio);
                cmd.Parameters.AddWithValue("HFIN", proyecto.Hfin);

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
                rd.Close();

                if (aux > 0)
                {
                    string hashedPassword = PasswordHelper.HashPassword(contra);

                    cmd = new MySqlCommand("NEWPWORD", cn);
                    cmd.Parameters.AddWithValue("NUEVA", hashedPassword);
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

        public UsuarioModel ValidarUsuario(string NOMBRECORREO)
        {
            conectar();
            UsuarioModel user = null;

            try
            {
                MySqlCommand cmd = new MySqlCommand("VALIDARUSUARIO", cn);
                cmd.Parameters.AddWithValue("NOMBRECORREO", NOMBRECORREO);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    user = new UsuarioModel
                    {
                        Id = Convert.ToInt32(rd["ID_USUARIO"]),
                        Nombre = rd["NOMBRE_USUARIO"].ToString(),
                        Rol = rd["NOMBRE_ROL"].ToString(),
                        Estado = Convert.ToBoolean(rd["ESTADO_USUARIO"]),
                        Contrasenia = rd["CONTRASENIA_USUARIO"] != DBNull.Value ? rd["CONTRASENIA_USUARIO"].ToString() : null
                    };
                }

                rd.Close();
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

        public int ContarUsuariosChairas()
        {
            conectar();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE NOMBRE_USUARIO LIKE 'Visitante Chairá%'", cn);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0; // Si hay algún error, devolvemos 0
            }
            finally
            {
                desconectar();
            }
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
                        ListaRecolectores = dr[15]+"",
                        Ubicacion = dr[16] +"",
                        Fecha = dr[17] +"",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false,
                        Division = dr[19] +"",
                        Proyecto = dr[20] +"",
                        Adultos = dr[21] +"",
                        Jovenes = dr[22] +"",
                        Condicion = dr[23] +"",
                        Origen = dr[24] +"",
                        Elevacionmin = dr[25] +"",
                        Elevacionmax = dr[26] +"",
                        Habitad = dr[27] +"",
                        ObervacionLocal = dr[28] +"",
                        Catalogo = dr[29] +"",
                        Registro = dr[30] +"",
                        Autor = dr[31] +""
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
                        ListaRecolectores = dr[15]+"",
                        Ubicacion = dr[16] + "",
                        Fecha = dr[17] + "",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false,
                        Division = dr[19] + "",
                        Proyecto = dr[20] + "",
                        Adultos = dr[21] + "",
                        Jovenes = dr[22] + "",
                        Condicion = dr[23] + "",
                        Origen = dr[24] + "",
                        Elevacionmin = dr[25] + "",
                        Elevacionmax = dr[26] + "",
                        Habitad = dr[27] + "",
                        ObervacionLocal = dr[28] + "",
                        Catalogo = dr[29] + "",
                        Registro = dr[30] + "",
                        Autor = dr[31] + ""
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

        public List<MuestraModel> ResultadoEspecifico(string sorden, string sclase, string sespecie, string sfamilia, string sgenero, string sdepa, string sdivision, string smuni)
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
                cmd.Parameters.AddWithValue("sdivision", sdivision);
                cmd.Parameters.AddWithValue("smuni", smuni);
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
                        ListaRecolectores = dr[15] + "",
                        Ubicacion = dr[16] + "",
                        Fecha = dr[17] + "",
                        Estado = Convert.ToBoolean(dr[18]) ? true : false,
                        Division = dr[19] + "",
                        Proyecto = dr[20] +"",
                        Adultos = dr[21] + "",
                        Jovenes = dr[22] + "",
                        Condicion = dr[23] + "",
                        Origen = dr[24] + "",
                        Elevacionmin = dr[25] + "",
                        Elevacionmax = dr[26] + "",
                        Habitad = dr[27] + "",
                        ObervacionLocal = dr[28] + "",
                        Catalogo = dr[29] + "",
                        Registro = dr[30] + "",
                        Autor = dr[31] + ""
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
                        Division = rd[11] + "",
                        Procedencia = rd[12] + "",
                        Venacion = rd[13] + "",
                        Forma = rd[14] + "",
                        Margen = rd[15] + "",
                        Ubicacion = rd[16] + "",
                        ListaRecolectores = rd[17] + "",
                        Fecha = rd[18]+"",
                        Proyecto = rd[19]+"",
                        Elevacionmin = rd[20]+"",
                        Elevacionmax = rd[21]+""

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
                cmd.Parameters.AddWithValue("ISCDA", ubi.Cuerpodeagua);

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

        public void EliminarComentario(int id){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("ELIMINARCOMENTARIO", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
        }

        public void EditarComentario(int id, string newcomentario){
            conectar();
            try{
                MySqlCommand cmd = new MySqlCommand("EDITARCOMENTARIO", cn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.Parameters.AddWithValue("NEWCOMENTARIO", newcomentario);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }finally{
                desconectar();
            }
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
                        Texto = dr[2]+"",
                        idUser = Convert.ToInt32(dr[3]+"")
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

        //Para la consulta de Darwin Core
        public DataTable ObtenerDatosInforme(DateTime inicio, DateTime fin)
        {
            conectar();
            DataTable dt = new DataTable();

            try
            {
                string query = "CALL DARWINCORE(@inicio, @fin)"; // Llamar el procedimiento almacenado
                MySqlCommand cmd = new MySqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fin", fin);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt); // Llenar el DataTable con los resultados
            }
            catch (Exception ex)
            {
                // Manejar la excepción según corresponda
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                desconectar();
            }

            return dt;
        }

        public byte[] GenerarInformeExcel(DataTable datos)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Informe");
                worksheet.Cell(1, 1).InsertTable(datos); // Insertar el DataTable en la hoja

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream); // Guardar el archivo Excel en un MemoryStream
                    return stream.ToArray(); // Devolver el archivo en formato byte[]
                }
            }
        }

        public void EditarParametros(string tabla, string parametro, string cambio)
        {
            conectar();
            try
            {
                // Llamada al procedimiento almacenado EDITARPARAMETROS
                MySqlCommand cmd = new MySqlCommand("EDITARPARAMETROS", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Agregar los parámetros al comando
                cmd.Parameters.AddWithValue("TABLA", tabla);
                cmd.Parameters.AddWithValue("PARAMETRO", parametro);
                cmd.Parameters.AddWithValue("CAMBIO", cambio);

                // Ejecutar el comando
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine(ex.Message);
            }
            finally
            {
                desconectar();
            }
        }

    }
}