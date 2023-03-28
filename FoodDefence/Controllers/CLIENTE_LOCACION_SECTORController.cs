using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodDefence.Models;


namespace FoodDefense.Controllers
{
    public class CLIENTE_LOCACION_SECTORController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: CLIENTE_LOCACION_SECTOR
        public ActionResult Index()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Include(c => c.CLIENTE_LOCACION).Include(c => c.CLIENTE_LOCACION.CLIENTE);
            return View(cLIENTE_LOCACION_SECTOR.ToList());
        }

        public ActionResult ListadoClienteLocacionSector(string pCliente = "", string pLocacion = "", string pSector = "", int pBaja = 0)
        {
            var cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Include(c => c.CLIENTE_LOCACION).Include(c => c.CLIENTE_LOCACION.CLIENTE);

            bool lBaja;
            if (pBaja >= 0)
            {
                if (pBaja == 0)
                {
                    lBaja = false;
                }
                else
                {
                    lBaja = true;
                }
                cLIENTE_LOCACION_SECTOR = cLIENTE_LOCACION_SECTOR.Where(n => n.baja == lBaja);
            }

            if (pCliente != "")
                cLIENTE_LOCACION_SECTOR = cLIENTE_LOCACION_SECTOR.Where(n => n.CLIENTE_LOCACION.CLIENTE.razonSocial.Contains(pCliente) || n.CLIENTE_LOCACION.CLIENTE.numeroDocumento.ToString().Contains(pCliente));

            if (pLocacion != "")
                cLIENTE_LOCACION_SECTOR = cLIENTE_LOCACION_SECTOR.Where(n => n.CLIENTE_LOCACION.descripcion.Contains(pLocacion));

            if (pSector != "")
                cLIENTE_LOCACION_SECTOR = cLIENTE_LOCACION_SECTOR.Where(n => n.descripcion.Contains(pSector));

            return PartialView("_ListadoClienteLocacionSector", cLIENTE_LOCACION_SECTOR);
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

        // GET: CLIENTE_LOCACION_SECTOR/Details/5
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
            CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Find(id);
            if (cLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            return View(cLIENTE_LOCACION_SECTOR);
        }

        // GET: CLIENTE_LOCACION_SECTOR/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION.Where(n => n.id == 0), "id", "descripcion");
            return View();
        }

        // POST: CLIENTE_LOCACION_SECTOR/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idClienteLocacion,descripcion")] CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR)
        {
            ViewBag.ValidacionesLocacionSector = "";
            if (db.CLIENTE_LOCACION_SECTOR.Where(c => c.baja==false && c.idClienteLocacion == cLIENTE_LOCACION_SECTOR.idClienteLocacion && c.descripcion == cLIENTE_LOCACION_SECTOR.descripcion).Count() > 0)
            {
                ViewBag.ValidacionesLocacionSector = "La locación " + db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().descripcion +
                                                     " del cliente " + db.CLIENTE.Where(m => m.id == db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente).FirstOrDefault().razonSocial +
                                                     " ya posee un sector con el nombre " + cLIENTE_LOCACION_SECTOR.descripcion + ".";

                if (cLIENTE_LOCACION_SECTOR.idClienteLocacion!=0)
                    ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente);
                else
                    ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
                ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION.Where(n => n.baja == false), "id", "descripcion", cLIENTE_LOCACION_SECTOR.idClienteLocacion);
                return View(cLIENTE_LOCACION_SECTOR);
            }

            if (ModelState.IsValid)
            {
                cLIENTE_LOCACION_SECTOR.baja = false;
                db.CLIENTE_LOCACION_SECTOR.Add(cLIENTE_LOCACION_SECTOR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (cLIENTE_LOCACION_SECTOR.idClienteLocacion != 0)
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente);
            else
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION, "id", "descripcion", cLIENTE_LOCACION_SECTOR.idClienteLocacion);
            return View(cLIENTE_LOCACION_SECTOR);
        }

        // GET: CLIENTE_LOCACION_SECTOR/Edit/5
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
            CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Find(id);
            if (cLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            if (cLIENTE_LOCACION_SECTOR.idClienteLocacion != 0)
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente);
            else
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION, "id", "descripcion", cLIENTE_LOCACION_SECTOR.idClienteLocacion);
            return View(cLIENTE_LOCACION_SECTOR);
        }

        // POST: CLIENTE_LOCACION_SECTOR/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idClienteLocacion,descripcion")] CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR)
        {
            ViewBag.ValidacionesLocacionSector = "";
            if (db.CLIENTE_LOCACION_SECTOR.Where(c => c.id != cLIENTE_LOCACION_SECTOR.id && c.idClienteLocacion == cLIENTE_LOCACION_SECTOR.idClienteLocacion && c.descripcion == cLIENTE_LOCACION_SECTOR.descripcion).Count() > 0)
            {
                ViewBag.ValidacionesLocacionSector = "La locación " + db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().descripcion +
                                                     " del cliente " + db.CLIENTE.Where(m => m.id == db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente).FirstOrDefault().razonSocial +
                                                     " ya posee un sector con el nombre " + cLIENTE_LOCACION_SECTOR.descripcion + ".";

                if (cLIENTE_LOCACION_SECTOR.idClienteLocacion != 0)
                    ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente);
                else
                    ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
                ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION, "id", "descripcion", cLIENTE_LOCACION_SECTOR.idClienteLocacion);
                return View(cLIENTE_LOCACION_SECTOR);
            }

            if (ModelState.IsValid)
            {
                db.Entry(cLIENTE_LOCACION_SECTOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (cLIENTE_LOCACION_SECTOR.idClienteLocacion != 0)
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", db.CLIENTE_LOCACION.Where(n => n.id == cLIENTE_LOCACION_SECTOR.idClienteLocacion).FirstOrDefault().idCliente);
            else
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial");
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION, "id", "descripcion", cLIENTE_LOCACION_SECTOR.idClienteLocacion);
            return View(cLIENTE_LOCACION_SECTOR);
        }

        // GET: CLIENTE_LOCACION_SECTOR/Delete/5
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
            CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Find(id);
            if (cLIENTE_LOCACION_SECTOR == null)
            {
                return HttpNotFound();
            }
            return View(cLIENTE_LOCACION_SECTOR);
        }

        // POST: CLIENTE_LOCACION_SECTOR/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CLIENTE_LOCACION_SECTOR cLIENTE_LOCACION_SECTOR = db.CLIENTE_LOCACION_SECTOR.Find(id);
            cLIENTE_LOCACION_SECTOR.baja = true;
            db.Entry(cLIENTE_LOCACION_SECTOR).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
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
