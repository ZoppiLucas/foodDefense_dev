using FoodDefence.Models;
using FoodDefence.Models.objectModel;
using FoodDefence.Models.Repository;
using FoodDefence.Models.Request;
using FoodDefence.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FoodDefence.Controllers
{
    [RoutePrefix("api/cargatrabajo")]
    public class rCargaTrabajoController : ApiController
    {
        [HttpGet]
        [Route("validartrampa/{idTrampaClienteLocacionSector}/{idEmpleado}")]
        public IHttpActionResult ValidarTrampa(int idTrampaClienteLocacionSector, int idEmpleado)
        {
            try
            {
                if (idTrampaClienteLocacionSector == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                List<validarTrampa_Result> oRes = oRep.validarTrampa(idTrampaClienteLocacionSector, idEmpleado).ToList();
                ValidacionTrampaResponse trampaResponse = new ValidacionTrampaResponse();
                trampaResponse.cantEncontradas = oRes.Count();

                ValidacionTrampa validacionTrampa = new ValidacionTrampa();
                foreach (var item in oRes)
                {
                    validacionTrampa = new ValidacionTrampa();
                    validacionTrampa.cargado = item.cargado == null ? false : (bool)item.cargado;
                    validacionTrampa.IdOrdenTrabajoDetalle = item.IdOrdenTrabajoDetalle;
                    validacionTrampa.IdOrdenTrabajo = item.idOrdenTrabajo;
                    trampaResponse.validacionTrampas.Add(validacionTrampa);
                }
                return Ok(trampaResponse);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("buscartrampa/{id}")]
        public IHttpActionResult BuscarTrampa(int id)
        
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();

                return Ok(oRep.armarDatosYCampoRequerido(id));
            }
            catch (Exception e)
            {                
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("guardar")]
        public IHttpActionResult Guardar([FromBody]TrampaRequest model)
        {

            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajoDetalle;

                if (model.idOrdenTrabajoDetalle == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }

                trampas_OrdenTrabajoDetalle = new Trampas_OrdenTrabajoDetalle
                {
                    idOrdenTrabajoDetalle = model.idOrdenTrabajoDetalle,
                    idAccion = model.idAccion,
                    idEstado = model.idEstado,
                    cucaAmericana = model.cucaAmericana,
                    cucaGermanica = model.cucaGermanica,
                    insecto = model.insecto,
                    mariposas = model.mariposas,
                    minusculos = model.minusculos,
                    moscas = model.moscas,
                    mosquitas = model.mosquitas,
                    polillas = model.polillas,
                    roedor = model.roedor,
                    cantidad = model.cantidad,
                    is_cantidad = model.is_cantidad,
                    is_cucaAmericana = model.is_cucaAmericana,
                    is_cucaGermanica = model.is_cucaGermanica,
                    is_idAccion = model.is_idAccion,
                    is_idEstado = model.is_idEstado,
                    is_insecto = model.is_insecto,
                    is_mariposas = model.is_mariposas,
                    is_minusculos = model.is_minusculos,
                    is_moscas = model.is_moscas,
                    is_mosquitas = model.is_mosquitas,
                    is_polillas = model.is_polillas,
                    is_roedor = model.is_roedor,
                    observaciones = model.observaciones,
                    idTipoTrampa = model.idTipoTrampa
                };                
                ResultOrdenTrabajoDetalle result = oRep.saveTrabajo(trampas_OrdenTrabajoDetalle);
                /* INSERT LOS ESTADO EN LA NUEVA TABLA */
                var lstEstados = new List <int>();
                lstEstados.Add( (int)model.idEstado) ;
                oRep.saveTrabajoEstados(model.idOrdenTrabajoDetalle, lstEstados);

                /*controlo si esta todo el trabajo cargado*/
                bool isTrabajoTodoCargado = oRep.isTrabajoTodoCargado(result.orden_trabajo_detalle.idOrdenTrabajo);
                /*controlo la cual es la ulitma trampa para cargar*/
                List<string> lastTrap;
                if (!isTrabajoTodoCargado)
                {
                    lastTrap = oRep.getLastTrap(result.orden_trabajo_detalle.id);
                }                    
                else
                {
                    lastTrap = new List<string>();
                }

                CargaResponse carga = new CargaResponse();
                carga.idOrdenTrabajo = result.orden_trabajo_detalle.idOrdenTrabajo;
                carga.idOrdenTrabajoDetalle = model.idOrdenTrabajoDetalle;
                carga.idTrampaClienteLocacionSector = result.orden_trabajo_detalle.idTrampaClienteLocacionSector;
                carga.success = result.success;
                carga.mensaje = result.errMsg;
                carga.workDate = getDateFormat(DateTime.Now);
                carga.workDateTime = DateTime.Now.ToString("dd/MM/yyyy H:mm");
                carga.lastTrap = lastTrap;
                carga.workCompleted = isTrabajoTodoCargado ? "S" : "N";
                carga.lstTareas = oRep.GetListAccionesTareas(
                                            result.orden_trabajo_detalle.idOrdenTrabajo, 
                                            trampas_OrdenTrabajoDetalle.idTipoTrampa, 
                                            model.idOrdenTrabajoDetalle);
                carga.mostrarAcciones = carga.lstTareas.Any();

                return Ok(carga);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }

        [HttpPost]
        [Route("GuardarWorkListTareas")]
        public IHttpActionResult GuardarWorkListTareas([FromBody] WorkListTareasInsert model)
        {
            CargaTrabajoRepository oRep = new CargaTrabajoRepository();
            try
            {
                bool rstSave = oRep.ListAccionesTareasInsert(model);
                return Ok(rstSave);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("completework/{idOrdenTrabajo}")]
        public IHttpActionResult CompleteWork(int idOrdenTrabajo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                ResultOrdenTrabajo result = oRep.finalizeTrabajo(idOrdenTrabajo);
                if (result.success)
                    return Ok("S");
                else
                    return Ok("N");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("getdatecorrectformat/{y}/{m}/{d}")]
        public IHttpActionResult getDateCorrectFormat(int y, int m, int d)
        {
            try
            {
                if (y == 0 || m == 0 || d == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }

                DateTime dateTime = new DateTime(y, m, d);
                string dateFormat = getDateFormat(dateTime);

                return Ok(dateFormat);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("getdatecorrectformatnow")]
        public IHttpActionResult getDateCorrectFormatNow()
        {
            try
            {
                string dateFormat = getDateFormat(DateTime.Now);

                return Ok(dateFormat);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        private string getDateFormat(DateTime dateTime)
        {

            //string y = dateTime.Year.ToString();
            //string m = dateTime.Month.ToString();
            //string d = dateTime.Day.ToString();

            return dateTime.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        [HttpGet]
        [Route("getclientesselect")]
        public IHttpActionResult getClientesSelect()
        {
            try
            {
                List<Select> lst = new List<Select>();
                 CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                lst = oRep.getClienteSelect();
                return Ok(lst);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        
        [HttpGet]
        [Route("getlocacionesselectbyidcliente/{id}")]
        public IHttpActionResult getLocacionesSelectByidCliente(int id)
        {
            try
            {
                List<SelectChild> lst = new List<SelectChild>();
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                lst = oRep.getLocacionesSelectByIdCliente(id);
                return Ok(lst);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("getlocacionesselect")]
        public IHttpActionResult getLocacionesSelect()
        {
            try
            {
                List<SelectChild> lst = new List<SelectChild>();
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                lst = oRep.getLocacionesSelect();
                return Ok(lst);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("gettrampatipo")]
        public IHttpActionResult getTrampaTipo()
        {
            try
            {
                List<Select> lst = new List<Select>();
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                lst = oRep.getTrampatipoSelect();                
                return Ok(lst);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("gettrampaapp/{idClienteLocacion}/{idTrampaTipo}/{numeroTrampa}/{idEmpleado}")]
        public IHttpActionResult getTrampaApp(int idClienteLocacion, int idTrampaTipo, string numeroTrampa, int idEmpleado)
        {
            try
            {
                List<int> lst = new List<int>();
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                lst = oRep.getTrampaApp(idClienteLocacion, idTrampaTipo, numeroTrampa, idEmpleado);
                TrampaAppResponse trampaAppResponse = new TrampaAppResponse();
                if (lst.Count() == 1)
                {
                    trampaAppResponse.idTrampaClienteLocacionSector = lst.First();                    
                }
                else {
                    if (lst.Count() > 1)
                    {
                        trampaAppResponse.idTrampaClienteLocacionSector = lst.First();
                    }
                    else
                    {
                        trampaAppResponse.idTrampaClienteLocacionSector = 0;                        
                    }
                }
                
                return Ok(trampaAppResponse);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("controlcantidadrequerida")]
        public IHttpActionResult controlCantidadRequerida([FromBody]ControlTrampaRequest model)
        {
            try
            {
                string result;
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                result = oRep.controlCantidadRequerida(model);
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }      
    }
}
