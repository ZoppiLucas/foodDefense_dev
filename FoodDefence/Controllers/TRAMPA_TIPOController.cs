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
    public class TRAMPA_TIPOController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: TRAMPA_TIPO
        public ActionResult Index()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            return View(db.TRAMPA_TIPO.Where(n=>n.baja==false).ToList());
        }

        // GET: TRAMPA_TIPO/Details/5
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
            TRAMPA_TIPO tRAMPA_TIPO = db.TRAMPA_TIPO.Find(id);
            if (tRAMPA_TIPO == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA_TIPO);
        }

        // GET: TRAMPA_TIPO/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            return View();
        }

        // POST: TRAMPA_TIPO/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] TRAMPA_TIPO tRAMPA_TIPO)
        {
            if (ModelState.IsValid)
            {
                tRAMPA_TIPO.baja = false;
                db.TRAMPA_TIPO.Add(tRAMPA_TIPO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tRAMPA_TIPO);
        }

        // GET: TRAMPA_TIPO/Edit/5
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
            TRAMPA_TIPO tRAMPA_TIPO = db.TRAMPA_TIPO.Find(id);
            if (tRAMPA_TIPO == null)
            {
                return HttpNotFound();
            }
            return View(tRAMPA_TIPO);
        }

        // POST: TRAMPA_TIPO/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] TRAMPA_TIPO tRAMPA_TIPO)
        {
            if (ModelState.IsValid)
            {
                tRAMPA_TIPO.baja = tRAMPA_TIPO.baja;
                db.Entry(tRAMPA_TIPO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAMPA_TIPO);
        }

        // GET: TRAMPA_TIPO/Delete/5
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
            TRAMPA_TIPO tRAMPA_TIPO = db.TRAMPA_TIPO.Find(id);
            if (tRAMPA_TIPO == null)
            {
                return HttpNotFound();
            }
            
            return View(tRAMPA_TIPO);
        }

        // POST: TRAMPA_TIPO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TRAMPA_TIPO tRAMPA_TIPO = db.TRAMPA_TIPO.Find(id);
            if (tRAMPA_TIPO.TRAMPA.Count == 0)
            {
                tRAMPA_TIPO.baja = true;
                db.Entry(tRAMPA_TIPO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(tRAMPA_TIPO);
            }
            
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
