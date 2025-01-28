using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using HDUA.DATA; // Asegúrate de importar el espacio de nombres adecuado
using HDUA.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

public class InformacionController : Controller
{
    [Authorize]
    public IActionResult CargarInformacion()
    {
        var conexion = ConexionMongo.Instance;
        var collection = conexion.Database.GetCollection<InformacionModel>("Informacion");
        var informacion = collection.Find(FilterDefinition<InformacionModel>.Empty).FirstOrDefault();

        return Json(informacion);
    }

    [HttpPost]
    public IActionResult ActualizarInformacion([FromBody] Dictionary<string, string> data)
    {
        try
        {
            var conexion = ConexionMongo.Instance;
            var collection = conexion.Database.GetCollection<InformacionModel>("Informacion");

            var filtro = Builders<InformacionModel>.Filter.Empty;

            var propertyName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data.Keys.First());

            var propertyInfo = typeof(InformacionModel).GetProperty(propertyName);

            if (propertyInfo == null)
            {
                return BadRequest($"La propiedad '{propertyName}' no existe en el modelo InformacionModel.");
            }

            var actualizacion = Builders<InformacionModel>.Update.Set(propertyName, data.Values.First());

            collection.UpdateOne(filtro, actualizacion);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al actualizar la información: " + ex.Message);
        }
    }





}