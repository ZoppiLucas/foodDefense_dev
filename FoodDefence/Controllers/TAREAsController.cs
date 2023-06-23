using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodDefence.Models;

namespace FoodDefence.Controllers
{
    public class TAREAsController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: TAREAs ---
        public ActionResult Index()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var tAREA = db.TAREA.Include(t => t.TRAMPA_TIPO);

            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View(tAREA.ToList());

        }

        public ActionResult ListadoTareas(string pDescripcion = "", int pTipoTrampa = 0)
        {
            var tAREAS = db.TAREA.Include(t => t.TRAMPA_TIPO);

            if (pTipoTrampa != 0)
                tAREAS = tAREAS.Where(n => n.idTipoTrampa == pTipoTrampa);            

            if (pDescripcion != "")
                tAREAS = tAREAS.Where(n => n.descripcion.Contains(pDescripcion));

            
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            
            return PartialView("_ListadoTareas", tAREAS);
        }


        // GET: TAREAs/Details/5
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
            TAREA tAREA = db.TAREA.Find(id);
            if (tAREA == null)
            {
                return HttpNotFound();
            }
            return View(tAREA);
        }

        // GET: TAREAs/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.ValidacionesTarea = "";

            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            return View();
        }

        // POST: TAREAs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idTipoTrampa,descripcion")] TAREA tAREA)
        {
            ViewBag.ValidacionesTarea = "";

            if (tAREA.descripcion == null) { tAREA.descripcion = ""; }
            else { tAREA.descripcion = tAREA.descripcion.Trim(); }
            if (tAREA.descripcion == "")
            {
                ViewBag.ValidacionesTarea = "Ingrese una descripción.";
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
                return View(tAREA);
            }

            if (db.TAREA.Where(n => n.descripcion == tAREA.descripcion && n.idTipoTrampa == tAREA.idTipoTrampa).Count() > 0)
            {
                ViewBag.ValidacionesTarea = "Ya existe una tarea con la misma descripción";
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
                return View(tAREA);
            }

            if (ModelState.IsValid)
            {
                db.TAREA.Add(tAREA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
            return View(tAREA);
        }

        // GET: TAREAs/Edit/5
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
            TAREA tAREA = db.TAREA.Find(id);
            if (tAREA == null)
            {
                return HttpNotFound();
            }
            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
            return View(tAREA);
        }

        // POST: TAREAs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idTipoTrampa,descripcion")] TAREA tAREA)
        {
            ViewBag.ValidacionesTarea = "";

            if (tAREA.descripcion == null) { tAREA.descripcion = ""; }
            else { tAREA.descripcion = tAREA.descripcion.Trim(); }
            if (tAREA.descripcion == "")
            {
                ViewBag.ValidacionesTarea = "Ingrese una descripción.";
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
                return View(tAREA);
            }

            if(db.TAREA.Where(n=>n.descripcion == tAREA.descripcion && n.idTipoTrampa == tAREA.idTipoTrampa).Count() > 0)
            {
                ViewBag.ValidacionesTarea = "Ya existe una tarea con la misma descripción";
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
                return View(tAREA);
            }

            if (ModelState.IsValid)
            {
                db.Entry(tAREA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", tAREA.idTipoTrampa);
            return View(tAREA);
        }

        // GET: TAREAs/Delete/5
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
            TAREA tAREA = db.TAREA.Find(id);
            if (tAREA == null)
            {
                return HttpNotFound();
            }
            return View(tAREA);
        }

        // POST: TAREAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TAREA tAREA = db.TAREA.Find(id);
            db.TAREA.Remove(tAREA);
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
