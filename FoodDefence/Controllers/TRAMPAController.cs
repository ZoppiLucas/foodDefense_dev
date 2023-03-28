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
    public class TRAMPAController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();
        #region vistas
        // GET: TRAMPA
        public ActionResult Index(int pEstado = 0)
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var tRAMPA = db.TRAMPA.Include(t => t.TRAMPA_ESTADO).Include(t => t.TRAMPA_TIPO);
            if (pEstado!=0)
                tRAMPA = db.TRAMPA.Include(t => t.TRAMPA_ESTADO).Include(t => t.TRAMPA_TIPO).Where(n=>n.idTrampaEstado==pEstado);

            ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion");
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View(tRAMPA.ToList());
        }
        
        public ActionResult ListadoTrampas(int pEstado = 0, string pNumero = "", int pTipoTrampa = 0)
        {
            var tRAMPA = db.TRAMPA.Include(t => t.TRAMPA_ESTADO).Include(t => t.TRAMPA_TIPO);

            if (pTipoTrampa != 0)
                tRAMPA = tRAMPA.Where(n => n.idTrampaTipo == pTipoTrampa);

            if (pNumero != "")
                tRAMPA = tRAMPA.Where(n => n.numero.Contains(pNumero));

            if (pEstado != 0)
                tRAMPA = tRAMPA.Where(n => n.idTrampaEstado == pEstado);

            //ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion");
            //ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");

            return PartialView("_ListadoTrampas", tRAMPA);
        }

        // GET: TRAMPA/Details/5
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
            TRAMPA tRAMPA = db.TRAMPA.Find(id);
            if (tRAMPA == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA);
        }

        // GET: TRAMPA/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.ValidacionesTrampa = "";

            ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion");
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View();
        }

        // POST: TRAMPA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idTrampaTipo,idTrampaEstado,numero,observaciones")] TRAMPA tRAMPA)
        {
            ViewBag.ValidacionesTrampa = "";
            if (db.TRAMPA.Where(n=> n.idTrampaTipo == tRAMPA.idTrampaTipo && n.numero==tRAMPA.numero).Count()>0)
            {
                ViewBag.ValidacionesTrampa = "Ya existe una trampa para el tipo " + db.TRAMPA_TIPO.Where(n=>n.id == tRAMPA.idTrampaTipo).FirstOrDefault().descripcion.ToString() + " con el número " + tRAMPA.numero.ToString() + ".";
                ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion", tRAMPA.idTrampaEstado);
                ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tRAMPA.idTrampaTipo);
                return View(tRAMPA);
            }

            if (ModelState.IsValid)
            {
                db.TRAMPA.Add(tRAMPA);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            
            ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion", tRAMPA.idTrampaEstado);
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tRAMPA.idTrampaTipo);
            return View(tRAMPA);
        }

        // GET: TRAMPA/Edit/5
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
            TRAMPA tRAMPA = db.TRAMPA.Find(id);
            if (tRAMPA == null)
            {
                return HttpNotFound();
            }
            ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion", tRAMPA.idTrampaEstado);
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tRAMPA.idTrampaTipo);
            return View(tRAMPA);
        }

        // POST: TRAMPA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idTrampaTipo,idTrampaEstado,numero,observaciones")] TRAMPA tRAMPA)
        {
            ViewBag.ValidacionesTrampa = "";
            if (db.TRAMPA.Where(n => n.idTrampaTipo == tRAMPA.idTrampaTipo && n.numero == tRAMPA.numero && n.id != tRAMPA.id).Count() > 0)
            {
                ViewBag.ValidacionesTrampa = "Ya existe una trampa para el tipo " + db.TRAMPA_TIPO.Where(n => n.id == tRAMPA.idTrampaTipo).FirstOrDefault().descripcion.ToString() + " con el número " + tRAMPA.numero.ToString() + ".";
                ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion", tRAMPA.idTrampaEstado);
                ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tRAMPA.idTrampaTipo);
                return View(tRAMPA);
            }

            if (ModelState.IsValid)
            {
                db.Entry(tRAMPA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idTrampaEstado = new SelectList(db.TRAMPA_ESTADO, "id", "descripcion", tRAMPA.idTrampaEstado);
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tRAMPA.idTrampaTipo);
            return View(tRAMPA);
        }

        // GET: TRAMPA/Delete/5
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
            TRAMPA tRAMPA = db.TRAMPA.Find(id);
            if (tRAMPA == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA);
        }

        // POST: TRAMPA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TRAMPA tRAMPA = db.TRAMPA.Find(id);

            if (ModelState.IsValid)
            {
                tRAMPA.idTrampaEstado = 3;
                db.Entry(tRAMPA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tRAMPA);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region JSON
        public JsonResult buscarTrampaTipoNumero(int pTipo = 0, string pNumero = "")
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<TRAMPA> lista = db.TRAMPA.Where(n => n.idTrampaTipo == pTipo && n.numero.ToUpper().Equals(pNumero.ToUpper()) && n.idTrampaEstado == 1).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
