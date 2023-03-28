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

namespace FoodDefense.Controllers
{
    public class ORDEN_TRABAJOController : Controller
    {


        // GET: ORDEN_TRABAJO
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            //var oRDEN_TRABAJO = db.ORDEN_TRABAJO.Include(o => o.CLIENTE_LOCACION).Include(o => o.EMPLEADO).Include(o => o.ORDEN_TRABAJO_ESTADO).FirstOrDefault();
            //oRDEN_TRABAJO.fechaCargaOrden = DateTime.Now;
            cleanSession();
            ViewBag.idOrdenTrabajoEstado = new SelectList(db.ORDEN_TRABAJO_ESTADO, "id", "descripcion");
            return View();
        }

        public ActionResult ListadoOrdenesTrabajo(string pCliente = ""
                                                , string pEmpleado = ""
                                                , string pFechaDesde = ""
                                                , string pFechaHasta = ""
                                                , string pEstado = "")
        {
            //var lFechaDesde = pFechaDesde != "" ? Convert.ToDateTime(pFechaDesde) : null;
            //var lFechaHasta = pFechaHasta != "" ? Convert.ToDateTime(pFechaHasta) : null;

            DateTime? lFechaDesde = null;
            DateTime? lFechaHasta = null;
            if (pFechaDesde != "")
                lFechaDesde = Convert.ToDateTime(pFechaDesde);
            if (pFechaHasta != "")
                lFechaHasta = Convert.ToDateTime(pFechaHasta);

            int? lEstado = null;             
            if (pEstado != "")
            {
                lEstado  = int.Parse(pEstado);
            }
            
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<OrdenesTrabajo_Result> lDatos = db.st_getOrdenesTrabajo(pCliente, pEmpleado, lFechaDesde, lFechaHasta, lEstado).ToList();
            
            return PartialView("_ListadoOrdenesTrabajo", lDatos);
        }

        // GET: ORDEN_TRABAJO/Details/5
        public ActionResult Details(int? id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            cleanSession();

            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }
            
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();
            
            string lstTrampaCargadas = "N";
            ViewBag.cantTrampas = 0;
            List <Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
            lstTrampa = getDetalle(oRDEN_TRABAJO.id);
            if (lstTrampa.Count > 0)
            {                
                ViewBag.lstTrampa = lstTrampa;
                ViewBag.cantTrampas = lstTrampa.Count;
                lstTrampaCargadas = "S";
            }
            ViewBag.lstTrampaCargadas = lstTrampaCargadas;
            
            if (oRDEN_TRABAJO.idEmpleado == null || oRDEN_TRABAJO.idEmpleado == 0)
            {
                ViewBag.Select_Empleado = "N";
            }
            else
            {
                ViewBag.Select_Empleado = "S";
                EMPLEADO eMPLEADO = db.EMPLEADO.Where(w => w.id == oRDEN_TRABAJO.idEmpleado).First();
                ViewBag.Empleado = eMPLEADO.apellido + ", " + eMPLEADO.nombre;
            }

            return View(oRDEN_TRABAJO);
        }

        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            var lOrdenTrabajo = new ORDEN_TRABAJO();
            lOrdenTrabajo.fechaCargaOrden = DateTime.Now;
            lOrdenTrabajo.fechaDeTrabajo = DateTime.Now;

            cleanSession();

            var empleados = db.EMPLEADO.Where(w => w.baja == false).Select(s => new
            {
                id = s.id,
                apellido = s.apellido + ", " + s.nombre
            }).ToList();
            ViewBag.idEmpleado = new SelectList(empleados, "id", "apellido");

            ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
            ViewBag.ValidacionesOrdenTrabajo = "";
            ViewBag.lstTrampa = "";
            ViewBag.cantTrampas = 0;
            return View(lOrdenTrabajo);
        }

        // POST: ORDEN_TRABAJO/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idOrdenTrabajoEstado,idClienteLocacion,fechaCargaOrden,fechaDeTrabajo,idEmpleado")] ORDEN_TRABAJO oRDEN_TRABAJO)
        {
            ViewBag.ValidacionesOrdenTrabajo = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {

                var empleados = db.EMPLEADO.Where(w => w.baja == false).Select(s => new
                {
                    id = s.id,
                    apellido = s.apellido + ", " + s.nombre
                }).ToList();
                ViewBag.idEmpleado = new SelectList(empleados, "id", "apellido");

                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
                ViewBag.ValidacionesOrdenTrabajo = "";
                string lstTrampaCargadas = "N";
                List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
                if (Session["lstTrampas_OrdenTrabajo"] != null)
                {
                    lstTrampa = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];
                    ViewBag.lstTrampa = lstTrampa;
                    if (lstTrampa.Count > 0)
                        lstTrampaCargadas = "S";
                }
                ViewBag.lstTrampaCargadas = lstTrampaCargadas;

                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            DateTime fechaCargaOrden = new DateTime(oRDEN_TRABAJO.fechaCargaOrden.Year, oRDEN_TRABAJO.fechaCargaOrden.Month, oRDEN_TRABAJO.fechaCargaOrden.Day, 0, 0, 0);
                            DateTime fechaDeTrabajo = new DateTime(oRDEN_TRABAJO.fechaDeTrabajo.Year, oRDEN_TRABAJO.fechaDeTrabajo.Month, oRDEN_TRABAJO.fechaDeTrabajo.Day, 0, 0, 0);
                            oRDEN_TRABAJO.fechaCargaOrden = fechaCargaOrden;
                            oRDEN_TRABAJO.fechaDeTrabajo = fechaDeTrabajo;
                            oRDEN_TRABAJO.idOrdenTrabajoEstado = 1;
                            db.ORDEN_TRABAJO.Add(oRDEN_TRABAJO);
                            db.SaveChanges();

                            foreach (var item in lstTrampa)
                            {
                                ORDEN_TRABAJO_DETALLE oDetalle = new ORDEN_TRABAJO_DETALLE();
                                oDetalle.idOrdenTrabajo = oRDEN_TRABAJO.id;
                                oDetalle.idTrampaClienteLocacionSector = item.idTrampaClienteLocacionSector;
                                oDetalle.cargado = false;
                                db.ORDEN_TRABAJO_DETALLE.Add(oDetalle);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesOrdenTrabajo = "Error al guardar la orden de trabajo, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesOrdenTrabajo = "Por favor controlar, existe algun dato erroneo.";

                }
                return View(oRDEN_TRABAJO);

            }

        }

        // GET: ORDEN_TRABAJO/Edit/5
        public ActionResult Edit(int? id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }

            var empleados = db.EMPLEADO.Where(w => w.baja == false).Select(s => new
            {
                id = s.id,
                apellido = s.apellido + ", " + s.nombre
            }).ToList();

            ViewBag.idEmpleado = new SelectList(empleados, "id", "apellido", oRDEN_TRABAJO.idEmpleado);
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();

            ViewBag.ValidacionesOrdenTrabajo = "";
            string lstTrampaCargadas = "N";
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
            lstTrampa = getDetalle(oRDEN_TRABAJO.id);

            cleanSession();

            if (lstTrampa.Count > 0)
            {
                Session["lstTrampas_OrdenTrabajo"] = lstTrampa;
                ViewBag.lstTrampa = lstTrampa;
                lstTrampaCargadas = "S";
            }
            
            ViewBag.lstTrampaCargadas = lstTrampaCargadas;

            return View(oRDEN_TRABAJO);
        }

        private List<Trampas_OrdenTrabajo> getDetalle(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            List<Trampas_OrdenTrabajo> lstTrampas = (from p in db.st_getDetalleTrampasOrdenTrabajo(id)
                                                     select new Trampas_OrdenTrabajo
                                                     {
                                                         numero = p.numero,
                                                         descripcion = p.descripcion,
                                                         idTrampaClienteLocacionSector = (int)p.idTrampaClienteLocacionSector,
                                                         nueva = "N",
                                                         descTipoTrampa = p.descTipoTrampa,
                                                         idTipoTrampa = (int)p.idTipoTrampa,
                                                         orden = (int)p.orden,
                                                     }).OrderBy(o => o.orden).ToList();
            return lstTrampas;
        }

        // POST: ORDEN_TRABAJO/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idOrdenTrabajoEstado,idClienteLocacion,fechaCargaOrden,fechaDeTrabajo,idEmpleado")] ORDEN_TRABAJO oRDEN_TRABAJO)
        {

            ViewBag.ValidacionesOrdenTrabajo = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                ORDEN_TRABAJO orden = db.ORDEN_TRABAJO.Find(oRDEN_TRABAJO.id);

                var empleados = db.EMPLEADO.Where(w => w.baja == false).Select(s => new
                {
                    id = s.id,
                    apellido = s.apellido + ", " + s.nombre
                }).ToList();
                ViewBag.idEmpleado = new SelectList(empleados, "id", "apellido");

                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);

                string lstTrampaCargadas = "N";
                List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
                if (Session["lstTrampas_OrdenTrabajo"] != null)
                {
                    lstTrampa = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];
                    ViewBag.lstTrampa = lstTrampa;
                    if (lstTrampa.Count > 0)
                        lstTrampaCargadas = "S";
                }
                ViewBag.lstTrampaCargadas = lstTrampaCargadas;

                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            DateTime fechaDeTrabajo = new DateTime(oRDEN_TRABAJO.fechaDeTrabajo.Year, oRDEN_TRABAJO.fechaDeTrabajo.Month, oRDEN_TRABAJO.fechaDeTrabajo.Day, 0, 0, 0);
                            orden.fechaDeTrabajo = fechaDeTrabajo;
                            orden.idEmpleado = oRDEN_TRABAJO.idEmpleado;
                            db.Entry(orden).State = EntityState.Modified;
                            List<ORDEN_TRABAJO_DETALLE> ordenes_borrar = db.ORDEN_TRABAJO_DETALLE.Where(w => w.idOrdenTrabajo == oRDEN_TRABAJO.id).ToList();
                            db.ORDEN_TRABAJO_DETALLE.RemoveRange(ordenes_borrar);

                            db.SaveChanges();

                            foreach (var item in lstTrampa)
                            {
                                ORDEN_TRABAJO_DETALLE oDetalle = new ORDEN_TRABAJO_DETALLE();
                                oDetalle.idOrdenTrabajo = oRDEN_TRABAJO.id;
                                oDetalle.idTrampaClienteLocacionSector = item.idTrampaClienteLocacionSector;
                                oDetalle.cargado = false;
                                db.ORDEN_TRABAJO_DETALLE.Add(oDetalle);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesOrdenTrabajo = "Error al guardar la orden de trabajo, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesOrdenTrabajo = "Por favor controlar, existe algun dato erroneo.";

                }
                return View(oRDEN_TRABAJO);

            }

        }

        // GET: ORDEN_TRABAJO/Delete/5
        public ActionResult Delete(int? id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();

            return View(oRDEN_TRABAJO);
        }

        // POST: ORDEN_TRABAJO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            oRDEN_TRABAJO.idOrdenTrabajoEstado = 4;
            db.Entry(oRDEN_TRABAJO).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int? id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
            if (oRDEN_TRABAJO == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Where(w => w.id == oRDEN_TRABAJO.idClienteLocacion).First();
            ViewBag.Cliente = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).First();

            if (oRDEN_TRABAJO.idEmpleado == null || oRDEN_TRABAJO.idEmpleado == 0)
            {
                ViewBag.Select_Empleado = "N";
                var empleados = db.EMPLEADO.Where(w => w.baja == false).Select(s => new
                {
                    id = s.id,
                    apellido = s.apellido + ", " + s.nombre
                }).ToList();
                ViewBag.idEmpleado = new SelectList(empleados, "id", "apellido");

            }
            else
            {
                ViewBag.Select_Empleado = "S";
                EMPLEADO eMPLEADO = db.EMPLEADO.Where(w => w.id == oRDEN_TRABAJO.idEmpleado).First();
                ViewBag.Empleado = eMPLEADO.apellido + ", " + eMPLEADO.nombre;
            }

            return View(oRDEN_TRABAJO);
        }


        public JsonResult AprobarOrden(int id, int idEmpleado)
        {
            try
            {
                FoodDefense_DevEntities db = new FoodDefense_DevEntities();
                ORDEN_TRABAJO oRDEN_TRABAJO = db.ORDEN_TRABAJO.Find(id);
                oRDEN_TRABAJO.idEmpleado = idEmpleado;
                oRDEN_TRABAJO.idOrdenTrabajoEstado = 2;
                db.Entry(oRDEN_TRABAJO).State = EntityState.Modified;
                db.SaveChanges();
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public JsonResult buscarTrampas(int pLocacion , string pNumero = "")
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<st_getTrampasOrdenTrabajo_Result> lstTrampas = db.st_getTrampasOrdenTrabajo("T", pLocacion, 0, 0, "", "").ToList();
            var lista = lstTrampas.Where(w => w.numero.ToUpper().StartsWith(pNumero.ToUpper())).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboTipoTrampa()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<TRAMPA_TIPO> lista = db.TRAMPA_TIPO.Where(w => w.baja == false).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult agregarTrampaXnumero(int idTrampaClienteLocacionSector, string idTrampaClienteLocacionSector_descripcion, string numero)
        {
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();

            if (Session["lstTrampas_OrdenTrabajo"] != null)
            {
                lstTrampa = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];
                lstTrampa.ForEach(w => w.nueva = "N");
            }

            if (lstTrampa.Count > 0)
            {
                foreach (var item in lstTrampa)
                {
                    if (item.idTrampaClienteLocacionSector == idTrampaClienteLocacionSector)
                    {
                        return Json("ERROR", JsonRequestBehavior.AllowGet);
                    }
                }
            }

            lstTrampa.Add(new Trampas_OrdenTrabajo
            {
                descripcion = idTrampaClienteLocacionSector_descripcion,
                idTrampaClienteLocacionSector = idTrampaClienteLocacionSector,
                numero = numero,
                nueva = "S"
            });

            Session["lstTrampas_OrdenTrabajo"] = lstTrampa;

            return Json(lstTrampa, JsonRequestBehavior.AllowGet);
        }


        public JsonResult agregarTrampaX(string pTrampaOpciones, int pLocacion, int pLocacionSector, int pTipo, string pTrampaDesde = "", string pTrampaHasta = "")
        {
            List<Trampas_OrdenTrabajo> lstTrampa_Anteriores = new List<Trampas_OrdenTrabajo>();

            if (Session["lstTrampas_OrdenTrabajo"] != null)
            {
                lstTrampa_Anteriores = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];
            }

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<st_getTrampasOrdenTrabajo_Result> lstTrampas_Result = db.st_getTrampasOrdenTrabajo(pTrampaOpciones, pLocacion, pLocacionSector, pTipo, pTrampaDesde, pTrampaHasta).ToList();

            List<Trampas_OrdenTrabajo> lstTrampas_Nuevas = (from p in lstTrampas_Result
                                                            select new Trampas_OrdenTrabajo
                                                            {
                                                                numero = p.numero,
                                                                descripcion = p.descripcion,
                                                                idTrampaClienteLocacionSector = (int)p.idTrampaClienteLocacionSector,
                                                                nueva = "S"
                                                            }).ToList();

            if (lstTrampa_Anteriores.Count > 0)
            {
                lstTrampa_Anteriores.ForEach(w => w.nueva = "N");
                List<Trampas_OrdenTrabajo> lstTrampaF = lstTrampa_Anteriores.Union(lstTrampas_Nuevas).ToList();
                Session["lstTrampas_OrdenTrabajo"] = lstTrampaF;
                return Json(lstTrampaF, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["lstTrampas_OrdenTrabajo"] = lstTrampas_Nuevas;
                return Json(lstTrampas_Nuevas, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult DeleteTrampaOrdenTrabajo(int trampaIndex)
        {
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();

            if (Session["lstTrampas_OrdenTrabajo"] != null)
            {
                lstTrampa = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];

                lstTrampa.RemoveAt(trampaIndex);
                Session["lstTrampas_OrdenTrabajo"] = lstTrampa;
            }
            else
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            return Json(lstTrampa, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAllTrampaOrdenTrabajo()
        {
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();

            if (Session["lstTrampas_OrdenTrabajo"] != null)
            {
                lstTrampa = (List<Trampas_OrdenTrabajo>)Session["lstTrampas_OrdenTrabajo"];

                if (lstTrampa.Count > 0)
                    lstTrampa.Clear();
                else
                    return Json("SIN_DATOS", JsonRequestBehavior.AllowGet);

                Session["lstTrampas_OrdenTrabajo"] = lstTrampa;
            }
            else
            {
                return Json("SIN_DATOS", JsonRequestBehavior.AllowGet);
            }
            return Json(lstTrampa, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsCarga(int? id)
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
            
            return View(oRDEN_TRABAJO);
        }

        public ActionResult DetailsAbreviaciones(int? id)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            List<TrampaControlDetallaAbreviaciones> lstTrampaControlDetallaAbreviaciones = new List<TrampaControlDetallaAbreviaciones>();
            CargaTrabajoRepository oRep = new CargaTrabajoRepository();
            lstTrampaControlDetallaAbreviaciones = oRep.getTrampaControlDetallaAbreviaciones();
            ViewBag.idOrdenTrabajo = id;
            return View(lstTrampaControlDetallaAbreviaciones);
        }

        public JsonResult getTrampas(int idOrdenTrabajo)
        {
            try
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                List<TipoTrampas_OrdenTrabajoDetalle> lst = oRep.getTrampasGroupTipos(idOrdenTrabajo);

                for (int i = 0; i < lst.Count; i++)
               {
                    //activar controles
                    List<string> controles = oRep.getCampoRequeridoCargaControlTrampa(lst[i].idTipoTrampa);

                    //idEstado
                    if (controles.Any(s => s.Contains("idEstado")))
                    {
                        lst[i].is_idEstado = "S";
                    }
                    //idAccion
                    if (controles.Any(s => s.Contains("idAccion")))
                    {
                        lst[i].is_idAccion = "S";
                    }
                    //moscas
                    if (controles.Any(s => s.Contains("moscas")))
                    {
                        lst[i].is_moscas = "S";
                    }
                    //mosquitas
                    if (controles.Any(s => s.Contains("mosquitas")))
                    {
                        lst[i].is_mosquitas = "S";
                    }
                    //polillas
                    if (controles.Any(s => s.Contains("polillas")))
                    {
                        lst[i].is_polillas = "S";
                    }
                    //mariposas
                    if (controles.Any(s => s.Contains("mariposas")))
                    {
                        lst[i].is_mariposas = "S";
                    }
                    //minusculos
                    if (controles.Any(s => s.Contains("minusculos")))
                    {
                        lst[i].is_minusculos = "S";
                    }
                    //roedor
                    if (controles.Any(s => s.Contains("roedor")))
                    {
                        lst[i].is_roedor = "S";
                    }
                    //insecto
                    if (controles.Any(s => s.Contains("insecto")))
                    {
                        lst[i].is_insecto = "S";
                    }
                    //cucaGermanica
                    if (controles.Any(s => s.Contains("cucaGermanica")))
                    {
                        lst[i].is_cucaGermanica = "S";
                    }
                    //cucaAmericana
                    if (controles.Any(s => s.Contains("cucaAmericana")))
                    {
                        lst[i].is_cucaAmericana = "S";
                    }
                    //cantidad
                    if (controles.Any(s => s.Contains("cantidad")))
                    {
                        lst[i].is_cantidad = "S";
                    }

                }                                
                

                //List<TipoTrampas_OrdenTrabajo> lst = oRep.getTrampaGroupTipos(idOrdenTrabajo);
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

        #region Exportar Datos
        public ActionResult ExporData()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();            
            cleanSession();

            if (Session["idCliente"] != null)
            {
                int lIdCliente = (int)Session["idCliente"];
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false && n.id == lIdCliente), "id", "razonSocial", null);
            }
            else
            {
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
            }
            GenericoRepository oGen = new GenericoRepository();
            List<Periodo>  listP = oGen.getPeriodosOrdenTrabajo();

            ViewBag.cmbPeriodo = new SelectList(listP, "key", "nombre", null);

            return View();
        }

        public ActionResult ExporDataCsv(int pCliente, int pLocacion, string pPeriodo)
        {            
            try
            {
                GenericoRepository oGen = new GenericoRepository();

                string[] p = pPeriodo.Split(';');
                int m = int.Parse(p[0]);
                int y = int.Parse(p[1]);
                DateTime date = new DateTime(y, m, 1);
                 
                string archivoCsv = oGen.exportarCSV(pCliente, pLocacion, date);

                DateTime fechaC = new DateTime();
                fechaC = DateTime.Now;
                string nombre;
                nombre = "Archivo_" + fechaC.Year.ToString() + fechaC.Month.ToString() + fechaC.Day.ToString() + fechaC.Hour.ToString() + fechaC.Minute.ToString() + fechaC.Second.ToString();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + nombre + ".csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(archivoCsv);
                Response.Flush();
                Response.End();


            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            return null;
        }
        
        #endregion


        public void cleanSession()
        {
            Session["lstTrampas_OrdenTrabajo"] = null;
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
