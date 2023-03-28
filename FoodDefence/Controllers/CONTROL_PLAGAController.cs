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

namespace FoodDefence.Controllers
{
    public class CONTROL_PLAGAController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: CONTROL_PLAGA
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var cONTROL_PLAGA = db.CONTROL_PLAGA.Include(c => c.CLIENTE_LOCACION).Include(c => c.TRAMPA_TIPO);
            List<CLIENTE> cLIENTES = db.CLIENTE.Where(n => n.baja == false).ToList();
            cLIENTES.Add(new CLIENTE { id = 0, razonSocial = "TODOS" });
            ViewBag.idCliente = new SelectList(cLIENTES.OrderBy(o => o.id), "id", "razonSocial", 0);
            ViewBag.idTrampaTipo = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");

            return View(cONTROL_PLAGA.ToList());
        }

        public ActionResult ListadoControlPlaga(string pCliente = "", string pClienteLocacion = "", int pTipoTrampa = 0, string pCampoCondicion = "")
        {
            List<st_GetListaControlPlaga_Result> listado = db.st_GetListaControlPlaga(pCliente, pClienteLocacion, pTipoTrampa, pCampoCondicion).ToList();
            ViewData["listado"] = listado;
            return PartialView("_ListadoControlPlaga");
        }

        // GET: CONTROL_PLAGA/Details/5
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

            CONTROL_PLAGA cONTROL_PLAGA = db.CONTROL_PLAGA.Find(id);
            if (cONTROL_PLAGA == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(cONTROL_PLAGA.idClienteLocacion);
            ViewBag.razonSocial = db.CLIENTE.Find(cLIENTE_LOCACION.idCliente).razonSocial;

            List<TAREA> tareasDetalles;
            tareasDetalles = (from tar in db.TAREA
                              join det in db.CONTROL_PLAGA_DETALLE on tar.id equals det.idTarea
                              where det.idControlPlaga == id
                              select tar).ToList();

            ViewBag.tareasDetalles = tareasDetalles.ToList();

            return View(cONTROL_PLAGA);
        }

        // GET: CONTROL_PLAGA/Create
        public ActionResult Create()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
            ViewBag.idTarea = new SelectList(db.TAREA.Where(n => n.idTipoTrampa == 1), "id", "descripcion");
            ViewBag.operadorCondicion = new SelectList(obternetOperador(), "id", "descripcion");

            return View();
        }

        // POST: CONTROL_PLAGA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idClienteLocacion,idTipoTrampa,campoCondicion,operadorCondicion,valorCondicion")]
                                    CONTROL_PLAGA cONTROL_PLAGA, FormCollection fConllection)
        {
            var idTareas = fConllection["idTarea"];
            var campoCondicion = fConllection["campoCondicion"];

            ViewBag.ValidacionesControlPlaga = "";
            if (string.IsNullOrEmpty(cONTROL_PLAGA.campoCondicion))
            {
                ViewBag.ValidacionesControlPlaga = "Ingrese una Campo Condición.";
            } else
            {
                if (cONTROL_PLAGA.campoCondicion.Trim().Length == 0)
                {
                    ViewBag.ValidacionesControlPlaga = "Ingrese una Campo Condición.";
                }
            }

            if (idTareas == null && ViewBag.ValidacionesControlPlaga == "")
            {
                ViewBag.ValidacionesControlPlaga = "Seleccione una Tarea.";
            }

            if (cONTROL_PLAGA.idClienteLocacion == 0)
            {
                ViewBag.ValidacionesControlPlaga = "Seleccione una Locación.";
            }

            if (ModelState.IsValid && ViewBag.ValidacionesControlPlaga == "")
            {
                db.CONTROL_PLAGA.Add(cONTROL_PLAGA);
                db.SaveChanges();
                var idNew = cONTROL_PLAGA.id;
                string[] tareas = idTareas.Split(',');

                foreach (var t in tareas)
                {
                    CONTROL_PLAGA_DETALLE cONTROL_PLAGA_DETALLE = new CONTROL_PLAGA_DETALLE();
                    cONTROL_PLAGA_DETALLE.idControlPlaga = idNew;
                    cONTROL_PLAGA_DETALLE.idTarea = Int32.Parse(t);
                    db.CONTROL_PLAGA_DETALLE.Add(cONTROL_PLAGA_DETALLE);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            } else
            {
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
                ViewBag.idTarea = new SelectList(db.TAREA.Where(n => n.idTipoTrampa == 1), "id", "descripcion");
                ViewBag.operadorCondicion = new SelectList(obternetOperador(), "id", "descripcion");

                return View(cONTROL_PLAGA);
            }


        }

        // GET: CONTROL_PLAGA/Edit/5
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
            CONTROL_PLAGA cONTROL_PLAGA = db.CONTROL_PLAGA.Find(id);
            if (cONTROL_PLAGA == null)
            {
                return HttpNotFound();
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(cONTROL_PLAGA.idClienteLocacion);
            ViewBag.idCliente = new SelectList(db.CLIENTE, "id", "razonSocial", cLIENTE_LOCACION.idCliente);
            ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION.Where(n => n.idCliente == cLIENTE_LOCACION.idCliente), "id", "descripcion", cONTROL_PLAGA.idClienteLocacion);

            ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion", cONTROL_PLAGA.idTipoTrampa);
            ViewBag.idTarea = new SelectList(db.TAREA.Where(n => n.idTipoTrampa == cONTROL_PLAGA.idTipoTrampa), "id", "descripcion");
            ViewBag.idTareaSelect = new SelectList(db.CONTROL_PLAGA_DETALLE.Where(d => d.idControlPlaga == id), "idTarea", "id");
            ViewBag.campoCondicion = new SelectList(ComboCamposControlEdit(cONTROL_PLAGA.idTipoTrampa), "campo", "campo", cONTROL_PLAGA.campoCondicion);
            if (cONTROL_PLAGA.campoCondicion == "Estado")
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                var rst = (from p in oRep.getTrampaControlEstado(cONTROL_PLAGA.idTipoTrampa)
                           select new
                           {
                               p.id,
                               descripcion = p.descripcion + '(' + p.abreviatura + ')'
                           }
                       ).ToList();
                ViewBag.operadorCondicion = new SelectList(rst, "id", "descripcion", cONTROL_PLAGA.operadorCondicion);
            } else
            {
                // ViewBag.campoCondicion = new SelectList(ComboCamposControlEdit(cONTROL_PLAGA.idTipoTrampa), "campo", "campo", cONTROL_PLAGA.campoCondicion);
                ViewBag.operadorCondicion = new SelectList(obternetOperador(), "id", "descripcion", cONTROL_PLAGA.operadorCondicion);
            }

            return View(cONTROL_PLAGA);
        }

        // POST: CONTROL_PLAGA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idClienteLocacion,idTipoTrampa,campoCondicion,operadorCondicion,valorCondicion")] CONTROL_PLAGA cONTROL_PLAGA, FormCollection fConllection)
        {
            var idTareas = fConllection["idTarea"];
            var idCP = cONTROL_PLAGA.id;
            ViewBag.ValidacionesControlPlaga = "";
            if (string.IsNullOrEmpty(cONTROL_PLAGA.campoCondicion))
            {
                ViewBag.ValidacionesControlPlaga = "Ingrese una Campo.";
            }
            else
            {
                if (cONTROL_PLAGA.campoCondicion.Trim().Length == 0)
                {
                    ViewBag.ValidacionesControlPlaga = "Ingrese una Campo.";
                }
            }

            if (idTareas == null && ViewBag.ValidacionesControlPlaga == "")
            {
                ViewBag.ValidacionesControlPlaga = "Seleccione una Tarea.";
            }

            if (ModelState.IsValid && ViewBag.ValidacionesControlPlaga == "")
            {
                db.Entry(cONTROL_PLAGA).State = EntityState.Modified;
                db.SaveChanges();
                string[] tareas = idTareas.Split(',');

                List<CONTROL_PLAGA_DETALLE> borrarControlPlagaDetalle = db.CONTROL_PLAGA_DETALLE.Where(n => n.idControlPlaga == idCP).ToList();
                db.CONTROL_PLAGA_DETALLE.RemoveRange(borrarControlPlagaDetalle);
                db.SaveChanges();

                foreach (var t in tareas)
                {
                    CONTROL_PLAGA_DETALLE cONTROL_PLAGA_DETALLE = new CONTROL_PLAGA_DETALLE();
                    cONTROL_PLAGA_DETALLE.idControlPlaga = idCP;
                    cONTROL_PLAGA_DETALLE.idTarea = Int32.Parse(t);
                    db.CONTROL_PLAGA_DETALLE.Add(cONTROL_PLAGA_DETALLE);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.idClienteLocacion = new SelectList(db.CLIENTE_LOCACION, "id", "descripcion");
                ViewBag.idTipoTrampa = new SelectList(db.TRAMPA_TIPO, "id", "descripcion");
                ViewBag.idTarea = new SelectList(db.TAREA.Where(n => n.idTipoTrampa == cONTROL_PLAGA.idTipoTrampa), "id", "descripcion");
                ViewBag.idTareaSelect = new SelectList(db.CONTROL_PLAGA_DETALLE.Where(d => d.idControlPlaga == idCP), "idTarea", "id");
                ViewBag.operadorCondicion = new SelectList(obternetOperador(), "id", "descripcion");

                return View(cONTROL_PLAGA);
            }
        }

        // GET: CONTROL_PLAGA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTROL_PLAGA cONTROL_PLAGA = db.CONTROL_PLAGA.Find(id);
            if (cONTROL_PLAGA == null)
            {
                return HttpNotFound();
            }

            if (cONTROL_PLAGA.campoCondicion == "Estado")
            {
                CargaTrabajoRepository oRep = new CargaTrabajoRepository();
                TRAMPA_CONTROL_ESTADO controlEstado = oRep.getTrampaControlEstadoById( int.Parse(cONTROL_PLAGA.operadorCondicion) );
                ViewBag.estado = controlEstado.descripcion + " (" + controlEstado.abreviatura + ")";
                ViewBag.verEstado = "S";
            } else
            {
                ViewBag.verEstado = "N";
            }

            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(cONTROL_PLAGA.idClienteLocacion);
            ViewBag.cliente = db.CLIENTE.Find(cLIENTE_LOCACION.idCliente).razonSocial;

            List<TAREA> tareasDetalles;
            tareasDetalles = (from tar in db.TAREA
                              join det in db.CONTROL_PLAGA_DETALLE on tar.id equals det.idTarea
                              where det.idControlPlaga == id
                              select tar).ToList();

            ViewBag.tareasDetalles = tareasDetalles.ToList();

            return View(cONTROL_PLAGA);
        }

        // POST: CONTROL_PLAGA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CONTROL_PLAGA cONTROL_PLAGA = db.CONTROL_PLAGA.Find(id);
            db.CONTROL_PLAGA.Remove(cONTROL_PLAGA);
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

        public ActionResult actTarea(int pIdTrampaTipo)
        {
            ViewBag.idTarea = new SelectList(db.TAREA.Where(n => n.idTipoTrampa == pIdTrampaTipo), "id", "descripcion");
            return PartialView("_ActTarea");
        }

        public List<ControlPlagaOperadores> obternetOperador()
        {
            var cTO = new List<ControlPlagaOperadores>();
            cTO.Add(new ControlPlagaOperadores { id = "<=", descripcion = "Menor Igual (<=)" });
            cTO.Add(new ControlPlagaOperadores { id = ">", descripcion = "Mayor (>)" });
            cTO.Add(new ControlPlagaOperadores { id = "<", descripcion = "Menor (<)" });
            cTO.Add(new ControlPlagaOperadores { id = ">=", descripcion = "Mayor Igul (>=)" });
            return cTO;
        }

        public JsonResult ComboCamposControl(int pTrampaTipo = 0)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<st_getCamposControlPlaga_Result> campoControl = db.st_getCamposControlPlaga(pTrampaTipo).ToList();
            return Json(campoControl, JsonRequestBehavior.AllowGet);
        }

        public List<st_getCamposControlPlaga_Result> ComboCamposControlEdit(int idTrampaTipo = 0)
        {
            List<st_getCamposControlPlaga_Result> campoControl = db.st_getCamposControlPlaga(idTrampaTipo).ToList();
            return campoControl;
        }

        public JsonResult cargarCondicionEstado(int pTipo = 0)
        {
            db.Configuration.ProxyCreationEnabled = false;            
            CargaTrabajoRepository oRep = new CargaTrabajoRepository();

            if (pTipo == 0)
            {
                var rst = obternetOperador();
                return Json(rst, JsonRequestBehavior.AllowGet);
            }
            else
            {
               var rst = (from p in oRep.getTrampaControlEstado(pTipo)
                           select new
                           {
                               p.id,
                               descripcion = p.descripcion + '(' + p.abreviatura + ')'
                           }
                        ).ToList();
                return Json(rst , JsonRequestBehavior.AllowGet);
            }            
        }

    }
}
