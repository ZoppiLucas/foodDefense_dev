using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodDefence.Models;
using FoodDefence.Models.objectModel;
using FoodDefence.Models.Repository;

namespace FoodDefense.Controllers
{
    public class TRAMPA_CLIENTE_LOCACION_SECTORController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: TRAMPA_CLIENTE_LOCACION_SECTOR
        public ActionResult Index()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var tRAMPA_CLIENTE_LOCACION_SECTOR = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Include(t => t.CLIENTE_LOCACION_SECTOR).Include(t => t.TRAMPA);
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR.ToList());
        }

        public ActionResult ListadoTrampaClienteLocacionSector(string pCliente = ""
                                                            , string pLocacion = ""
                                                            , string pSector = ""
                                                            , int pTipotrampa = 0
                                                            , string pNumero = "")
        {
            var lDatos = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Include(t => t.CLIENTE_LOCACION_SECTOR).Include(t => t.TRAMPA);

            lDatos = lDatos.Where(n => n.fechaHasta == null);

            if (pCliente != "")
                lDatos  = lDatos .Where(n => n.CLIENTE_LOCACION_SECTOR.CLIENTE_LOCACION.CLIENTE.razonSocial.Contains(pCliente) || n.CLIENTE_LOCACION_SECTOR.CLIENTE_LOCACION.CLIENTE.numeroDocumento.ToString().Contains(pCliente));

            if (pLocacion != "")
                lDatos  = lDatos .Where(n => n.CLIENTE_LOCACION_SECTOR.CLIENTE_LOCACION.descripcion.Contains(pLocacion));

            if (pSector != "")
                lDatos  = lDatos .Where(n => n.CLIENTE_LOCACION_SECTOR.descripcion.Contains(pSector));

            if (pTipotrampa != 0)
                lDatos = lDatos.Where(n => n.TRAMPA.idTrampaTipo==pTipotrampa);

            if (pNumero != "")
                lDatos = lDatos.Where(n => n.TRAMPA.numero.Contains(pNumero));

            
            return PartialView("_ListadoTrampaClienteLocacionSector", lDatos );
        }


        public JsonResult ComboClienteLocacion(int pCliente = 0)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<CLIENTE_LOCACION> lista;
            if (pCliente > 0)
                lista = db.CLIENTE_LOCACION.Where(n => n.baja == false && n.idCliente == pCliente).ToList();
            else
                lista = db.CLIENTE_LOCACION.Where(n => n.baja == false && n.id == 0).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboClienteLocacionSector(int pLocacion = 0)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<CLIENTE_LOCACION_SECTOR> lista;
            if (pLocacion > 0)
                lista = db.CLIENTE_LOCACION_SECTOR.Where(n => n.baja == false && n.idClienteLocacion == pLocacion).ToList();
            else
                lista = db.CLIENTE_LOCACION_SECTOR.Where(n => n.baja == false && n.id == 0).ToList();

            return Json(lista.OrderBy(o => o.descripcion), JsonRequestBehavior.AllowGet);
        }

        // GET: TRAMPA_CLIENTE_LOCACION_SECTOR/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Find(id);
            if (tRAMPA_CLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        // GET: TRAMPA_CLIENTE_LOCACION_SECTOR/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION.Where(n => n.baja == false), "id", "descripcion", null);
            ViewBag.idClienteLocacionSector = new SelectList(db.CLIENTE_LOCACION_SECTOR.Where(n => n.baja == false), "id", "descripcion", null);
            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", null);
            ViewBag.ValidacionesTrampaClienteLocacionSector = "";
            return View();
        }

        // POST: TRAMPA_CLIENTE_LOCACION_SECTOR/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idClienteLocacionSector,idTrampa,fechaDesde,fechaHasta,observacion")] TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR)
        {
            ViewBag.ValidacionesTrampaClienteLocacionSector = "";
            if (tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector == 0)
            {
                ViewBag.ValidacionesTrampaClienteLocacionSector = "Debe de seleccionar un sector.";
            }
            if (tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa == 0)
            {
                ViewBag.ValidacionesTrampaClienteLocacionSector = "La trampa seleccionada no existe o no se encuentra en estado Activa.";
            }
            else
            {
                TRAMPA_CLIENTE_LOCACION_SECTOR lTrampaAsociada = (from tcls in db.TRAMPA_CLIENTE_LOCACION_SECTOR
                                                                    where tcls.idTrampa == tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa
                                                                      && tcls.fechaHasta == null
                                                                    select tcls).FirstOrDefault();

                if (lTrampaAsociada != null)
                {
                    ViewBag.ValidacionesTrampaClienteLocacionSector = "La trampa tipo " + lTrampaAsociada.TRAMPA.TRAMPA_TIPO.descripcion + " y número " + lTrampaAsociada.TRAMPA.numero + " se encuentra asociada al cliente " +
                                                                        lTrampaAsociada.CLIENTE_LOCACION_SECTOR.CLIENTE_LOCACION.CLIENTE.razonSocial + " en la locación " + lTrampaAsociada.CLIENTE_LOCACION_SECTOR.CLIENTE_LOCACION.descripcion + " y el sector " +
                                                                        lTrampaAsociada.CLIENTE_LOCACION_SECTOR.descripcion + " desde la fecha " + lTrampaAsociada.fechaDesde.ToString("dd/MM/yyyy") + ".";
                }
            }

            if (ViewBag.ValidacionesTrampaClienteLocacionSector == "")
            {
                tRAMPA_CLIENTE_LOCACION_SECTOR.fechaDesde = DateTime.Now;
                if (ModelState.IsValid)
                {
                    tRAMPA_CLIENTE_LOCACION_SECTOR.fechaDesde = DateTime.Now;
                    db.TRAMPA_CLIENTE_LOCACION_SECTOR.Add(tRAMPA_CLIENTE_LOCACION_SECTOR);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false)
                                                   , "id"
                                                   , "razonSocial"
                                                   , db.CLIENTE_LOCACION_SECTOR.Where(n=>n.id==tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector).FirstOrDefault().CLIENTE_LOCACION.idCliente);


            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION.Where(n => n.baja == false)
                                                        , "id"
                                                        , "descripcion"
                                                        , db.CLIENTE_LOCACION_SECTOR.Where(n => n.id == tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector).FirstOrDefault().idClienteLocacion);

            ViewBag.idClienteLocacionSector = new SelectList(db.CLIENTE_LOCACION_SECTOR.Where(n => n.baja == false)
                                                                , "id"
                                                                , "descripcion"
                                                                , tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector);

            int lTipoTrampa = 0;
            if (tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa != 0)
            {
                lTipoTrampa = db.TRAMPA.Where(n => n.id == tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa).FirstOrDefault().idTrampaTipo;
            }
            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", lTipoTrampa);

            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        // GET: TRAMPA_CLIENTE_LOCACION_SECTOR/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Find(id);
            if (tRAMPA_CLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            ViewBag.idClienteLocacionSector = new SelectList(db.CLIENTE_LOCACION_SECTOR, "id", "descripcion", tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector);
            ViewBag.idTrampa = new SelectList(db.TRAMPA, "id", "observaciones", tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa);
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        // POST: TRAMPA_CLIENTE_LOCACION_SECTOR/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idClienteLocacionSector,idTrampa,fechaDesde,fechaHasta,observacion")] TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAMPA_CLIENTE_LOCACION_SECTOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idClienteLocacionSector = new SelectList(db.CLIENTE_LOCACION_SECTOR, "id", "descripcion", tRAMPA_CLIENTE_LOCACION_SECTOR.idClienteLocacionSector);
            ViewBag.idTrampa = new SelectList(db.TRAMPA, "id", "observaciones", tRAMPA_CLIENTE_LOCACION_SECTOR.idTrampa);
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        // GET: TRAMPA_CLIENTE_LOCACION_SECTOR/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Find(id);
            if (tRAMPA_CLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        // POST: TRAMPA_CLIENTE_LOCACION_SECTOR/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TRAMPA_CLIENTE_LOCACION_SECTOR tRAMPA_CLIENTE_LOCACION_SECTOR = db.TRAMPA_CLIENTE_LOCACION_SECTOR.Find(id);
            tRAMPA_CLIENTE_LOCACION_SECTOR.fechaHasta = DateTime.Now;
            if (ModelState.IsValid)
            {
                tRAMPA_CLIENTE_LOCACION_SECTOR.fechaHasta = DateTime.Now;
                db.Entry(tRAMPA_CLIENTE_LOCACION_SECTOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAMPA_CLIENTE_LOCACION_SECTOR);
        }

        public JsonResult buscarTrampaTipoNumero(int pTipo = 0, string pNumero = "")
        {
            List<AutocompleteTrampas> autocompleteTrampas = new List<AutocompleteTrampas>();
            CargaTrabajoRepository oRep = new CargaTrabajoRepository();
            autocompleteTrampas = oRep.autocompleteTrampas(pNumero, pTipo);

            return Json(autocompleteTrampas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintStickers()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View();
        }

        public ActionResult ListadoTrampaClienteLocacionSectorImpresion(string pCliente = ""
                                                            , string pLocacion = ""
                                                            , string pSector = ""
                                                            , int pTipotrampa = 0
                                                            , string pNumero = "")
        {

            GenericoRepository oRep = new GenericoRepository();
            
            List<ListadoTrampaClienteLocacionSector_Result> lDatos = oRep.getListadoTrampaClienteLocacionSector(pCliente,pLocacion,pSector,pNumero,pTipotrampa);
            return PartialView("_ListadoTrampaClienteLocacionSectorImpresion", lDatos);
        }

        public ActionResult imprimirQRtrampas()
        {
            GenericoRepository oRep = new GenericoRepository();
            List<TrampaQR> trampas = (List<TrampaQR>)Session["QRtrampas"];

            //List<string> trampas = new List<string>();
            //trampas = pTrampas.Split(',').ToList();
            byte[] byteArray = oRep.imprimirQRtrampas(trampas);
            MemoryStream ms = new MemoryStream(byteArray);
            return new FileStreamResult(ms, "application/pdf");
        }

        public JsonResult cargarQRtrampas(List<TrampaQR> pTrampas)
        {
            Session["QRtrampas"] = pTrampas;

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
