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


namespace FoodDefense.Controllers
{
    public class CLIENTE_LOCACIONController : Controller
    {


        // GET: CLIENTE_LOCACION
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var cLIENTE_LOCACION = db.CLIENTE_LOCACION.Include(c => c.CLIENTE).Include(c => c.PROVINCIA);
            return View(cLIENTE_LOCACION.ToList());
        }

        public ActionResult ListadoClienteLocacion(string pCliente = "", string pLocacion = "", string pLocalidad = "", int pBaja = 0)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            var cLIENTE_LOCACION = db.CLIENTE_LOCACION.Include(c => c.CLIENTE).Include(c => c.PROVINCIA);

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
                cLIENTE_LOCACION = cLIENTE_LOCACION.Where(n => n.baja == lBaja);
            }

            if (pCliente != "")
                cLIENTE_LOCACION = cLIENTE_LOCACION.Where(n => n.CLIENTE.razonSocial.Contains(pCliente) || n.CLIENTE.numeroDocumento.ToString().Contains(pCliente));

            if (pLocacion != "")
                cLIENTE_LOCACION = cLIENTE_LOCACION.Where(n => n.descripcion.Contains(pLocacion));

            if (pLocalidad != "")
                cLIENTE_LOCACION = cLIENTE_LOCACION.Where(n => n.localidad.Contains(pLocalidad));

            return PartialView("_ListadoClienteLocacion", cLIENTE_LOCACION);
        }

        // GET: CLIENTE_LOCACION/Details/5
        public ActionResult Details(int? id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");
            cleanSession();            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(id);
            if (cLIENTE_LOCACION == null)
            {
                return HttpNotFound();
            }

            var ClienteNombre = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).FirstOrDefault();
            ViewBag.ClienteNombre = ClienteNombre;
            var ProvinciaNombre = db.PROVINCIA.Where(w => w.id == cLIENTE_LOCACION.idProvincia).Select(s => s.descripcion).FirstOrDefault();
            ViewBag.ProvinciaNombre = ProvinciaNombre;
            List<Responsable> lstResponsables = new List<Responsable>();
            lstResponsables = getResponsable(cLIENTE_LOCACION.id);
            ViewBag.lstResponsables = lstResponsables;
            Session["lstResponsables"] = lstResponsables;

            return View(cLIENTE_LOCACION);
        }
        [HttpGet]
        public ActionResult ResponsableDetails(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            Responsable oResponsable = new Responsable();
            List<Contactos> lstContacos = new List<Contactos>();

            List<Responsable> lstResponsables = new List<Responsable>();
            if (Session["lstResponsables"] != null)
                lstResponsables = (List<Responsable>)Session["lstResponsables"];

            lstContacos = lstResponsables[id].contactos;
            oResponsable.apellido = lstResponsables[id].apellido;
            oResponsable.nombre = lstResponsables[id].nombre;
            oResponsable.habilitadoPortal = lstResponsables[id].habilitadoPortal;
            oResponsable.usuarioPortal = lstResponsables[id].usuarioPortal;
            oResponsable.clavePortal = lstResponsables[id].clavePortal;


            ViewBag.lstContactosResponsable = lstContacos;

            return PartialView(oResponsable);
        }

        [HttpGet]
        public ActionResult Responsable(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            Responsable oResponsable = new Responsable();
            List<Contactos> lstContacos = new List<Contactos>();
            string habitlitadoPortal;
            habitlitadoPortal = "S";
            if (id >= 0)
            {
                List<Responsable> lstResponsables = new List<Responsable>();
                if (Session["lstResponsables"] != null)
                    lstResponsables = (List<Responsable>)Session["lstResponsables"];

                lstContacos = lstResponsables[id].contactos;
                oResponsable.apellido = lstResponsables[id].apellido;
                oResponsable.nombre = lstResponsables[id].nombre;
                oResponsable.habilitadoPortal = lstResponsables[id].habilitadoPortal;
                oResponsable.usuarioPortal = lstResponsables[id].usuarioPortal;
                oResponsable.clavePortal = lstResponsables[id].clavePortal;
                oResponsable.isEdit = true;
                oResponsable.isEdit_Index = id;
                habitlitadoPortal = lstResponsables[id].habilitadoPortal ? "S" : "N";
            }
            else
            {
                oResponsable.isEdit = false;
                oResponsable.isEdit_Index = -1;
            }


            // lstContacos.Add(new Contactos { ContactoTipo = "pepe", idContacto = 1, idContactoTipo = 1, observaciones = "sin", valor = "545" });
            ViewBag.lstContactosResponsable = lstContacos;
            ViewBag.habitlitadoPortal = habitlitadoPortal;

            var lstTiposContactos = db.CONTACTO_TIPO.ToList();
            ViewBag.tiposContactos = new SelectList(lstTiposContactos, "id", "descripcion");
            return PartialView(oResponsable);
        }

        [HttpPost]
        public JsonResult SaveResponsable(Responsable model)
        {

            List<Responsable> lstResponsables = new List<Responsable>();
            if (Session["lstResponsables"] != null)
                lstResponsables = (List<Responsable>)Session["lstResponsables"];

            for (int i = 0; i < lstResponsables.Count; i++)
            {
                if (lstResponsables[i].nombre.Trim().ToLower() == model.nombre.Trim().ToLower() &&
                    lstResponsables[i].apellido.Trim().ToLower() == model.apellido.Trim().ToLower() &&
                    i != model.isEdit_Index)
                    return Json("", JsonRequestBehavior.AllowGet);
            }

            if (model.isEdit)
            {
                lstResponsables[model.isEdit_Index].apellido = model.apellido;
                lstResponsables[model.isEdit_Index].nombre = model.nombre;
                lstResponsables[model.isEdit_Index].habilitadoPortal = model.habilitadoPortal;
                lstResponsables[model.isEdit_Index].usuarioPortal = model.usuarioPortal;
                lstResponsables[model.isEdit_Index].clavePortal = model.clavePortal;
                lstResponsables[model.isEdit_Index].contactos = model.contactos;
                lstResponsables[model.isEdit_Index].isEdit = false;
                lstResponsables[model.isEdit_Index].isEdit_Index = -1;
            }
            else
            {
                lstResponsables.Add(new Responsable
                {
                    apellido = model.apellido,
                    nombre = model.nombre,
                    habilitadoPortal = model.habilitadoPortal,
                    usuarioPortal = model.usuarioPortal,
                    clavePortal = model.clavePortal,
                    contactos = model.contactos,
                    isEdit = false,
                    isEdit_Index = -1
                });
            }
            Session["lstResponsables"] = lstResponsables;
            return Json(lstResponsables, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResponsable(int id)
        {
            List<Responsable> lstResponsables = new List<Responsable>();

            if (Session["lstResponsables"] != null)
            {
                lstResponsables = (List<Responsable>)Session["lstResponsables"];

                lstResponsables.RemoveAt(id);
                Session["lstResponsables"] = lstResponsables;
            }
            else
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            return Json(lstResponsables, JsonRequestBehavior.AllowGet);
        }

        public List<Responsable> getResponsable(int idClienteLocacion)
        {
            List<Responsable> lstResponsables = new List<Responsable>();

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            var responsables = db.CLIENTE_LOCACION_RESPONSABLE.Where(w => w.idClienteLocacion == idClienteLocacion).ToList();

            for (int i = 0; i < responsables.Count(); i++)
            {
                //lstResponsables[i].contactos = responsables[i].contactos;
                int idCLR = responsables[i].id;
                var lstContactosResponsable = db.CLIENTE_LOCACION_RESPONSABLE_CONTACTO.Where(w => w.idClienteLocacionResponsable == idCLR).ToList();
                List<Contactos> contactos = new List<Contactos>();
                for (int e = 0; e < lstContactosResponsable.Count; e++)
                {
                    int idTC = lstContactosResponsable[e].idContactoTipo;
                    string ContactoTipo = db.CONTACTO_TIPO.Where(w => w.id == idTC ).Select(s => s.descripcion).First();
                    contactos.Add(new Contactos
                    {
                        idContacto = lstContactosResponsable[e].id,
                        observaciones = lstContactosResponsable[e].observaciones,
                        valor = lstContactosResponsable[e].valor,
                        idContactoTipo = lstContactosResponsable[e].idContactoTipo,
                        ContactoTipo = ContactoTipo
                    });
                }

                lstResponsables.Add(new Responsable
                {
                    apellido = responsables[i].apellido,
                    nombre = responsables[i].nombre,
                    habilitadoPortal = responsables[i].habilitadoPortal,
                    usuarioPortal = responsables[i].usuarioPortal,
                    clavePortal = responsables[i].clavePortal,
                    contactos = contactos,
                    isEdit = false,
                    isEdit_Index = -1
                });

            }
            return lstResponsables;
        }

        // GET: CLIENTE_LOCACION/Create
        public ActionResult Create()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");
            cleanSession();
            ViewBag.ValidacionesLocacion = "";

            var comboCliente = db.CLIENTE.Where(w => w.baja == false).ToList();
            ViewBag.idCliente = new SelectList(comboCliente, "id", "razonSocial");
            ViewBag.idProvincia = new SelectList(db.PROVINCIA, "id", "descripcion");
            return View();
        }

        // POST: CLIENTE_LOCACION/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idCliente,descripcion,idProvincia,localidad,calle,numero,piso,departamento,observaciones,cantidadDiasCebaderas")] CLIENTE_LOCACION cLIENTE_LOCACION)
        {
            ViewBag.ValidacionesLocacion = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                var comboCliente = db.CLIENTE.Where(w => w.baja == false).ToList();
                var comboProvincia = db.PROVINCIA.ToList();
                ViewBag.idCliente = new SelectList(comboCliente, "id", "razonSocial", cLIENTE_LOCACION.idCliente);
                ViewBag.idProvincia = new SelectList(comboProvincia, "id", "descripcion", cLIENTE_LOCACION.idProvincia);

                if (db.CLIENTE_LOCACION.Where(n => n.idCliente == cLIENTE_LOCACION.idCliente && n.descripcion == cLIENTE_LOCACION.descripcion).Count() > 0)
                {
                    ViewBag.ValidacionesLocacion = "Ya existe una una locación para el cliente " + db.CLIENTE.Where(n => n.id == cLIENTE_LOCACION.idCliente).FirstOrDefault().razonSocial.ToString() + " con la descripción " + cLIENTE_LOCACION.descripcion.ToString() + ".";
                    return View(cLIENTE_LOCACION);
                }

                if (ModelState.IsValid)
                {

                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            cLIENTE_LOCACION.baja = false;
                            db.CLIENTE_LOCACION.Add(cLIENTE_LOCACION);
                            db.SaveChanges();

                            List<Responsable> lstResponsables = new List<Responsable>();
                            if (Session["lstResponsables"] != null)
                                lstResponsables = (List<Responsable>)Session["lstResponsables"];
                            foreach (var item in lstResponsables)
                            {

                                CLIENTE_LOCACION_RESPONSABLE oResponsable = new CLIENTE_LOCACION_RESPONSABLE();
                                oResponsable.idClienteLocacion = cLIENTE_LOCACION.id;
                                oResponsable.apellido = item.apellido;
                                oResponsable.nombre = item.nombre;
                                oResponsable.habilitadoPortal = item.habilitadoPortal;
                                oResponsable.usuarioPortal = item.usuarioPortal;
                                oResponsable.clavePortal = item.clavePortal;
                                db.CLIENTE_LOCACION_RESPONSABLE.Add(oResponsable);
                                db.SaveChanges();

                                foreach (var itemContacto in item.contactos)
                                {
                                    CLIENTE_LOCACION_RESPONSABLE_CONTACTO oContacto = new CLIENTE_LOCACION_RESPONSABLE_CONTACTO();
                                    oContacto.idClienteLocacionResponsable = oResponsable.id;
                                    oContacto.idContactoTipo = itemContacto.idContactoTipo;
                                    oContacto.valor = itemContacto.valor;
                                    oContacto.observaciones = itemContacto.observaciones;
                                    db.CLIENTE_LOCACION_RESPONSABLE_CONTACTO.Add(oContacto);
                                }


                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesCliente = "Error al guardar la locacion, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }


                }
                else
                {
                    ViewBag.ValidacionesLocacion = "Por favor controlar, existe algun dato erroneo.";
                }

                return View(cLIENTE_LOCACION);
            }
        }


        // GET: CLIENTE_LOCACION/Edit/5
        public ActionResult Edit(int? id)
        {

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");
            cleanSession();
            ViewBag.ValidacionesLocacion = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(id);
            if (cLIENTE_LOCACION == null)
            {
                return HttpNotFound();
            }

            var ClienteNombre = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).FirstOrDefault();
            ViewBag.ClienteNombre = ClienteNombre;
            var cboProvincia = db.PROVINCIA.ToList();
            ViewBag.idProvincia = new SelectList(cboProvincia, "id", "descripcion", cLIENTE_LOCACION.idProvincia);
            List<Responsable> lstResponsables = new List<Responsable>();
            lstResponsables = getResponsable(cLIENTE_LOCACION.id);
            ViewBag.lstResponsables = lstResponsables;
            Session["lstResponsables"] = lstResponsables;

            return View(cLIENTE_LOCACION);


        }

        // POST: CLIENTE_LOCACION/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idCliente,descripcion,idProvincia,localidad,calle,numero,piso,departamento,observaciones,cantidadDiasCebaderas")] CLIENTE_LOCACION cLIENTE_LOCACION)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                if (Session["idUsuario"] == null)
                    return RedirectToAction("Ingreso", "Ingresar");
                if (Convert.ToInt32(Session["idUsuario"]) == 0)
                    return RedirectToAction("Ingreso", "Ingresar");

                var ClienteNombre = db.CLIENTE.Where(w => w.id == cLIENTE_LOCACION.idCliente).Select(s => s.razonSocial).FirstOrDefault();
                ViewBag.ClienteNombre = ClienteNombre;
                var cboProvincia = db.PROVINCIA.ToList();
                ViewBag.idProvincia = new SelectList(cboProvincia, "id", "descripcion", cLIENTE_LOCACION.idProvincia);

                List<Responsable> lstResponsables = new List<Responsable>();
                if (Session["lstResponsables"] != null)
                    lstResponsables = (List<Responsable>)Session["lstResponsables"];
                ViewBag.lstResponsables = lstResponsables;

                if (db.CLIENTE_LOCACION.Where(n => n.idCliente == cLIENTE_LOCACION.idCliente && n.descripcion.Trim().ToLower() == cLIENTE_LOCACION.descripcion.Trim().ToLower() && n.id != cLIENTE_LOCACION.id).Count() > 0)
                {
                    ViewBag.ValidacionesLocacion = "Ya existe una una locación para el cliente " + db.CLIENTE.Where(n => n.id == cLIENTE_LOCACION.idCliente).FirstOrDefault().razonSocial.ToString() + " con la descripción " + cLIENTE_LOCACION.descripcion.ToString() + ".";
                    return View(cLIENTE_LOCACION);
                }

                if (ModelState.IsValid)
                {
                    CLIENTE_LOCACION locacion = db.CLIENTE_LOCACION.Find(cLIENTE_LOCACION.id);

                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            locacion.idProvincia = cLIENTE_LOCACION.idProvincia;
                            locacion.localidad = cLIENTE_LOCACION.localidad;
                            locacion.calle = cLIENTE_LOCACION.calle;
                            locacion.numero = cLIENTE_LOCACION.numero;
                            locacion.piso = cLIENTE_LOCACION.piso;
                            locacion.departamento = cLIENTE_LOCACION.departamento;
                            locacion.observaciones = cLIENTE_LOCACION.observaciones;
                            locacion.descripcion = cLIENTE_LOCACION.descripcion;
                            locacion.cantidadDiasCebaderas = cLIENTE_LOCACION.cantidadDiasCebaderas;

                            db.Entry(locacion).State = EntityState.Modified;
                            // db.SaveChanges();
                            List<CLIENTE_LOCACION_RESPONSABLE> Responsables_borrar = db.CLIENTE_LOCACION_RESPONSABLE.Where(w => w.idClienteLocacion == cLIENTE_LOCACION.id).ToList();
                            foreach (var item in Responsables_borrar)
                            {
                                List<CLIENTE_LOCACION_RESPONSABLE_CONTACTO> ResponsablesContactos_borrar = db.CLIENTE_LOCACION_RESPONSABLE_CONTACTO.Where(w => w.idClienteLocacionResponsable == item.id).ToList();
                                db.CLIENTE_LOCACION_RESPONSABLE_CONTACTO.RemoveRange(ResponsablesContactos_borrar);
                            }

                            db.CLIENTE_LOCACION_RESPONSABLE.RemoveRange(Responsables_borrar);
                            db.SaveChanges();

                            if (lstResponsables.Count > 0)
                            {
                                foreach (var item in lstResponsables)
                                {
                                    CLIENTE_LOCACION_RESPONSABLE oResponsable = new CLIENTE_LOCACION_RESPONSABLE();
                                    oResponsable.idClienteLocacion = cLIENTE_LOCACION.id;
                                    oResponsable.apellido = item.apellido;
                                    oResponsable.nombre = item.nombre;
                                    oResponsable.habilitadoPortal = item.habilitadoPortal;
                                    oResponsable.usuarioPortal = item.usuarioPortal;
                                    oResponsable.clavePortal = item.clavePortal;
                                    db.CLIENTE_LOCACION_RESPONSABLE.Add(oResponsable);
                                    db.SaveChanges();

                                    foreach (var itemContacto in item.contactos)
                                    {
                                        CLIENTE_LOCACION_RESPONSABLE_CONTACTO oContacto = new CLIENTE_LOCACION_RESPONSABLE_CONTACTO();
                                        oContacto.idClienteLocacionResponsable = oResponsable.id;
                                        oContacto.idContactoTipo = itemContacto.idContactoTipo;
                                        oContacto.valor = itemContacto.valor;
                                        oContacto.observaciones = itemContacto.observaciones;
                                        db.CLIENTE_LOCACION_RESPONSABLE_CONTACTO.Add(oContacto);
                                    }
                                }
                                db.SaveChanges();
                            }

                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesLocacion = "Error al guardar el responsable, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesLocacion = "Por favor controlar, existe algun dato erroneo.";
                }

            }

            return View(cLIENTE_LOCACION);

        }

        // GET: CLIENTE_LOCACION/Delete/5
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
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(id);
            if (cLIENTE_LOCACION == null)
            {
                return HttpNotFound();
            }
            return View(cLIENTE_LOCACION);
        }

        // POST: CLIENTE_LOCACION/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            CLIENTE_LOCACION cLIENTE_LOCACION = db.CLIENTE_LOCACION.Find(id);
            cLIENTE_LOCACION.baja = true;
            db.Entry(cLIENTE_LOCACION).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reactivar(int? id)
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
            CLIENTE_LOCACION cLIENTE = db.CLIENTE_LOCACION.Find(id);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
            return View(cLIENTE);
        }


        [HttpPost, ActionName("Reactivar")]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivarConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            CLIENTE_LOCACION cLIENTE = db.CLIENTE_LOCACION.Find(id);
            cLIENTE.baja = false;
            db.Entry(cLIENTE).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public void cleanSession()
        {
            Session["lstResponsables"] = null;
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
