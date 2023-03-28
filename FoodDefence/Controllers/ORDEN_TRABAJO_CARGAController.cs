using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodDefence.Models;
using FoodDefence.Models.objectModel;
using FoodDefence.Models.Repository;
using FoodDefence.Models.Request;

namespace FoodDefense.Controllers
{
    public class ORDEN_TRABAJO_CARGAController : Controller
    {


        // GET: ORDEN_TRABAJO_CARGA
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            cleanSession();
            return View();
        }

        public ActionResult ListadoOrdenesTrabajo(string pCliente = ""
                                                , string pEmpleado = ""
                                                , string pFechaDesde = ""
                                                , string pFechaHasta = ""
                                                )
        {

            DateTime? lFechaDesde = null;
            DateTime? lFechaHasta = null;
            if (pFechaDesde != "")
                lFechaDesde = Convert.ToDateTime(pFechaDesde);
            if (pFechaHasta != "")
                lFechaHasta = Convert.ToDateTime(pFechaHasta);


            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<OrdenesTrabajo_Result> lDatos = db.st_getOrdenesTrabajo(pCliente, pEmpleado, lFechaDesde, lFechaHasta, 2).ToList();

            return PartialView("_ListadoOrdenesTrabajo", lDatos);
        }


        public ActionResult Create(int id)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            cleanSession();
            ViewBag.ValidacionesOrdenTrabajo = "";

            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();

            EMPLEADO eMPLEADO = db.EMPLEADO.Where(w => w.id == oRDEN_TRABAJO.idEmpleado).First();
            ViewBag.Empleado = eMPLEADO.apellido + ", " + eMPLEADO.nombre;

            return View(oRDEN_TRABAJO);
        }

        public JsonResult ComboTipoTrampa()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<TRAMPA_TIPO> lista = db.TRAMPA_TIPO.Where(w => w.baja == false).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getTrampas(int idOrdenTrabajo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                List<TipoTrampas_OrdenTrabajo> lst = oRep.getTrampaGroupTipos(idOrdenTrabajo);
                if (lst.Count > 0)
                {
                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("VACIO", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public JsonResult getTrampasById(int id, string pNumeroTrampa, int pTipo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                Trampas_OrdenTrabajo oTrampa = oRep.getTrampaByNumero(id, pNumeroTrampa, pTipo);


                if (oTrampa != null)
                {

                    return Json(oTrampa, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("VACIO", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
                throw;
            }

        }


        [HttpGet]
        public ActionResult CargaTrabajo(int id)
        {
            Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajo = new Trampas_OrdenTrabajoDetalle();
            trampas_OrdenTrabajo = armarControl("C",id);
            return PartialView(trampas_OrdenTrabajo);
        }

        [HttpGet]
        public ActionResult VerTrabajo(int id)
        {
            Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajo = new Trampas_OrdenTrabajoDetalle();
            trampas_OrdenTrabajo = armarControl("V", id);
            return PartialView(trampas_OrdenTrabajo);
        }

        public Trampas_OrdenTrabajoDetalle armarControl(string tipoVista, int id) {
            //tipoVista = "C" cargar "V" ver

            Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajo = new Trampas_OrdenTrabajoDetalle();
            CargaTrabajoRepository oRep = new CargaTrabajoRepository();
            trampas_OrdenTrabajo = oRep.getTrampaDetalle(id);
            //activar controles
            List<string> controles = oRep.getCampoRequeridoCargaControlTrampa(trampas_OrdenTrabajo.idTipoTrampa);

            if (tipoVista == "V") {
                //idEstado
                if (controles.Any(s => s.Contains("idEstado")))
                {
                    trampas_OrdenTrabajo.is_idEstado = "S";
                    TRAMPA_CONTROL_ESTADO controlEstado = oRep.getTrampaControlEstadoById((int)trampas_OrdenTrabajo.idEstado);
                    trampas_OrdenTrabajo.descripcionEstado = controlEstado.descripcion + "(" + controlEstado.abreviatura + ")";
                }
                //idAccion
                if (controles.Any(s => s.Contains("idAccion")))
                {
                    trampas_OrdenTrabajo.is_idAccion = "S";
                    TRAMPA_CONTROL_ACCION controlAccion = oRep.getTrampaControlAccionById((int)trampas_OrdenTrabajo.idAccion);
                    trampas_OrdenTrabajo.descripcionAccion = controlAccion.descripcion + "(" + controlAccion.abreviatura + ")";
                }
            }
            if (tipoVista == "C") {
                //idEstado
                if (controles.Any(s => s.Contains("idEstado")))
                {
                    trampas_OrdenTrabajo.is_idEstado = "S";
                    var lst = (from p in oRep.getTrampaControlEstado(trampas_OrdenTrabajo.idTipoTrampa)
                               select new
                               {
                                   p.id,
                                   descripcion = p.descripcion + '(' + p.abreviatura + ')'
                               }
                              ).ToList();

                    trampas_OrdenTrabajo.cmbEstado = new SelectList(lst, "id", "descripcion");
                    FoodDefense_DevEntities db = new FoodDefense_DevEntities();
                    List <int> lstSelect = (from est in db.TRAMPA_CONTROL_ESTADO
                                              join det in db.ORDEN_TRABAJO_DETALLE_ESTADO on est.id equals det.idEstado
                                              where det.idOrdenTrabajoDetalle == id
                                              select est.id).ToList() ;
                    trampas_OrdenTrabajo.listEstado = lstSelect;
                    if (lstSelect.Count() > 0 && trampas_OrdenTrabajo.multiSelTipoTrampa == false)
                    {
                        trampas_OrdenTrabajo.idEstado = lstSelect[0];
                    }                    
                }
                //idAccion
                if (controles.Any(s => s.Contains("idAccion")))
                {
                    trampas_OrdenTrabajo.is_idAccion = "S";
                    var lst = (from p in oRep.getTrampaControlAccion(trampas_OrdenTrabajo.idTipoTrampa)
                               select new
                               {
                                   p.id,
                                   descripcion = p.descripcion + '(' + p.abreviatura + ')'
                               }
                                      ).ToList();
                    trampas_OrdenTrabajo.cmbAccion = new SelectList(lst, "id", "descripcion", trampas_OrdenTrabajo.idAccion);
                }
            }
            //moscas
            if (controles.Any(s => s.Contains("moscas")))
            {
                trampas_OrdenTrabajo.is_moscas = "S";
            }
            //mosquitas
            if (controles.Any(s => s.Contains("mosquitas")))
            {
                trampas_OrdenTrabajo.is_mosquitas = "S";
            }
            //polillas
            if (controles.Any(s => s.Contains("polillas")))
            {
                trampas_OrdenTrabajo.is_polillas = "S";
            }
            //mariposas
            if (controles.Any(s => s.Contains("mariposas")))
            {
                trampas_OrdenTrabajo.is_mariposas = "S";
            }
            //minusculos
            if (controles.Any(s => s.Contains("minusculos")))
            {
                trampas_OrdenTrabajo.is_minusculos = "S";
            }
            //roedor
            if (controles.Any(s => s.Contains("roedor")))
            {
                trampas_OrdenTrabajo.is_roedor = "S";
            }
            //insecto
            if (controles.Any(s => s.Contains("insecto")))
            {
                trampas_OrdenTrabajo.is_insecto = "S";
            }
            //cucaGermanica
            if (controles.Any(s => s.Contains("cucaGermanica")))
            {
                trampas_OrdenTrabajo.is_cucaGermanica = "S";
            }
            //cucaAmericana
            if (controles.Any(s => s.Contains("cucaAmericana")))
            {
                trampas_OrdenTrabajo.is_cucaAmericana = "S";
            }
            //cantidad
            if (controles.Any(s => s.Contains("cantidad")))
            {
                trampas_OrdenTrabajo.is_cantidad = "S";
            }

            return trampas_OrdenTrabajo;

        }

        public JsonResult guardarTrabajo(Trampas_OrdenTrabajoDetalle model)
        {
            try
            {
                //control de trampas
                #region control de trampas                
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                string resultControl;
                ControlTrampaRequest modelControl = new ControlTrampaRequest();
                modelControl = new ControlTrampaRequest {
                    idAccion = model.idAccion,
                    // idEstado = model.idEstado,
                    idOrdenTrabajoDetalle = model.idOrdenTrabajoDetalle,
                    cucaAmericana = model.cucaAmericana,
                    cucaGermanica = model.cucaGermanica,
                    insecto = model.insecto,
                    mariposas = model.mariposas,
                    minusculos = model.minusculos,
                    moscas = model.moscas,
                    mosquitas = model.mosquitas,
                    polillas = model.polillas,
                    roedor = model.roedor,
                    observaciones = model.observaciones,
                    cantidad = model.cantidad                    
                };
                resultControl = oRep.controlCantidadRequerida(modelControl);
                if (!String.IsNullOrEmpty(resultControl))
                    return Json(resultControl, JsonRequestBehavior.AllowGet);
                #endregion              
                ResultOrdenTrabajoDetalle result = oRep.saveTrabajo(model);
                if (result.success)
                {
                    /*-- ESTADOS --*/
                    List<int> lst = new List<int>();
                    if (model.idEstado != null)
                    {
                        lst.Add((int)model.idEstado);                        
                    } else
                    {
                        lst = model.listEstado;
                    }
                    ResultOrdenTrabajo rstEstado = oRep.saveTrabajoEstados(model.idOrdenTrabajoDetalle, lst);

                    //ResultOrdenTrabajo rstEstado = oRep.saveTrabajoEstados(model.idOrdenTrabajoDetalle, model.listEstado);

                    if (!rstEstado.success)                        
                        return Json(result.errMsg, JsonRequestBehavior.AllowGet);    
                    else 
                        return Json("OK", JsonRequestBehavior.AllowGet);
                }else
                    return Json(result.errMsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public ActionResult Details(int? id, string finalizar)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            cleanSession();

            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();

            EMPLEADO eMPLEADO = db.EMPLEADO.Where(w => w.id == oRDEN_TRABAJO.idEmpleado).First();
            ViewBag.Empleado = eMPLEADO.apellido + ", " + eMPLEADO.nombre;
            ViewBag.Finalizar = finalizar;
            return View(oRDEN_TRABAJO);
        }



        public JsonResult controlTrampasCargadas(int idOrdenTrabajo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                bool isTrabajoTodoCargado = oRep.isTrabajoTodoCargado(idOrdenTrabajo);
                if (isTrabajoTodoCargado)
                    return Json("FINALIZAR", JsonRequestBehavior.AllowGet);
                else
                    return Json("SEGUIR", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult finalizarTrabajo(int idOrdenTrabajo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                ResultOrdenTrabajo result = oRep.finalizeTrabajo(idOrdenTrabajo);
                if (result.success)
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                    return Json(result.errMsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult borrarTrabajo(int idOrdenTrabajoDetalle)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                ResultOrdenTrabajoDetalle result = oRep.restartTrabajo(idOrdenTrabajoDetalle);
                if (result.success)
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                    return Json(result.errMsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public void cleanSession()
        {
            Session["lstTrampas_OrdenTrabajo"] = null;
        }

    }
}
