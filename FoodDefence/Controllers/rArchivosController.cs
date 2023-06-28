using FoodDefence.Models;
using System.Web.Http;
using FoodDefence.Models.Repository;
using FoodDefence.Models.objectModel;

namespace FoodDefence.Controllers
{
    [RoutePrefix("api/archivo")]
    public class rArchivosController : ApiController
    {
        // FoodDefense_DevEntities db = new FoodDefense_DevEntities();


        [HttpPost]
        [Route("arcCargar")]
        public IHttpActionResult arcCargar([FromBody] ArchivoBase datos)
        {
            try
            {
                FoodDefense_DevEntities db = new FoodDefense_DevEntities();
                //-- CONTROL DE USUARIO Y PASSWORD
                //-- VERSION
                //datos.datos = datos.datos.Replace("'", @"""");
                //JObject json = JObject.Parse(datos.datos);
                if (datos.version == 1)
                {
                    //ArchivoList lst = Newtonsoft.Json.JsonConvert.DeserializeObject<ArchivoList>(datos.datos);
                    var listDatos = datos.datos.Split(';');

                    if (listDatos.Length == datos.cantRegistros)
                    {
                        //-- GUARDAR
                        var arcCab = new ARCHIVOS_CABECERA();
                        arcCab.fechaCarga = System.DateTime.Now;
                        arcCab.arcVersion = datos.version;
                        arcCab.cantRegistros = datos.cantRegistros;
                        db.ARCHIVOS_CABECERA.Add(arcCab);
                        db.SaveChanges();

                        var arcDet = new ARCHIVOS_DETALLE();
                        arcDet.idArchivosCabecera = arcCab.id;
                        arcDet.datos = datos.datos;
                        db.ARCHIVOS_DETALLE.Add(arcDet);
                        db.SaveChanges();

                        return Ok("0");// OK SIN ERRORES.
                    }
                    else
                    {
                        return Ok("2");// ERROR CANTIDAD DE REGISTROS.
                    }
                }
                return Ok("3");// ERROR VERSION
            }
            catch
            {
                return Ok("1"); // ERROR DESCONOSIDO.
            }
        }

        [HttpPost]
        [Route("getEncryptar")]
        public IHttpActionResult getEncryptar([FromBody] ArchivosSecurity txt)
        {
            try
            {
                Security sec = new Security();
                return Ok(sec.Encrypt(txt.dato));
            }
            catch
            {
                return Ok("N");
            }
        }


    }


}
