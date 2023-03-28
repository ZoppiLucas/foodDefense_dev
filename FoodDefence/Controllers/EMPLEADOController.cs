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
    public class EMPLEADOController : Controller
    {
        

        // GET: EMPLEADO
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.idDocumentoTipo = new SelectList(db.DOCUMENTO_TIPO, "id", "descripcion");
            var eMPLEADO = db.EMPLEADO.Include(c => c.DOCUMENTO_TIPO);
            return View(eMPLEADO.ToList());
        }

        public ActionResult ListadoEmpleado(int pTipoDocumento = 0, long pNumeroDocumento = 0, string lApellido = "", int pBaja = 0)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            var eMPLEADO = db.EMPLEADO.Include(c => c.DOCUMENTO_TIPO);

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
                eMPLEADO = eMPLEADO.Where(n => n.baja == lBaja);
            }

            if (pTipoDocumento != 0)
                eMPLEADO = eMPLEADO.Where(n => n.idDocumentoTipo == pTipoDocumento);

            if (pNumeroDocumento != 0)
                eMPLEADO = eMPLEADO.Where(n => n.numeroDocumento.ToString().Contains(pNumeroDocumento.ToString()));

            if (lApellido != "")
                eMPLEADO = eMPLEADO.Where(n => n.apellido.Contains(lApellido));

            return PartialView("_ListadoEmpleado", eMPLEADO);
        }

        // GET: EMPLEADO/Details/5
        public ActionResult Details(int? id)
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
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            if (eMPLEADO == null)
            {
                return HttpNotFound();
            }
            cleanSession();

            //buscar Contactos
            ContactoResult lstContacto = new ContactoResult();
            lstContacto.contactos = getContactos(eMPLEADO.id);
            Session["lstContactoEmp"] = lstContacto.contactos;
            lstContacto.success = 1;
            ViewBag.lstContacto = lstContacto;
            string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == eMPLEADO.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
            ViewBag.DocumentoTipo = nombre;

            return View(eMPLEADO);
        }

        // GET: EMPLEADO/Create
        public ActionResult Create()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            cleanSession();

            ViewBag.ValidacionesEmpleado = "";
            ViewBag.idDocumentoTipo = new SelectList(db.DOCUMENTO_TIPO, "id", "descripcion");
            ViewBag.tiposContactos = new SelectList(db.CONTACTO_TIPO, "id", "descripcion");
            ViewBag.lstContacto = "";
            return View();
        }

        // POST: EMPLEADO/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idDocumentoTipo,numeroDocumento,apellido,nombre,baja")] EMPLEADO eMPLEADO)
        {
            ViewBag.ValidacionesEmpleado = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {

                var comboDocumentos = db.DOCUMENTO_TIPO.ToList();
                ViewBag.idDocumentoTipo = new SelectList(comboDocumentos, "id", "descripcion", eMPLEADO.idDocumentoTipo);

                var comboTiposContactos = db.CONTACTO_TIPO.ToList();
                ViewBag.tiposContactos = new SelectList(comboTiposContactos, "id", "descripcion");

                var lEmpleado = db.EMPLEADO.Where(n => n.idDocumentoTipo == eMPLEADO.idDocumentoTipo && n.numeroDocumento == eMPLEADO.numeroDocumento).Count();

                //buscar Contactos
                ContactoResult lstContactoVBag = new ContactoResult();
                if (Session["lstContactoEmp"] != null)
                {
                    lstContactoVBag.contactos = (List<Contactos>)Session["lstContactoEmp"];
                }

                lstContactoVBag.success = 1;
                ViewBag.lstContacto = lstContactoVBag;

                if (lEmpleado > 0)
                {
                    ViewBag.ValidacionesEmpleado = "Ya existe un Técnico para el documento " + db.DOCUMENTO_TIPO.Where(n => n.id == eMPLEADO.idDocumentoTipo).FirstOrDefault().descripcion.ToString() + " " + eMPLEADO.numeroDocumento.ToString() + ".";
                    return View(eMPLEADO);
                }

                eMPLEADO.baja = false;
                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.EMPLEADO.Add(eMPLEADO);
                            db.SaveChanges();

                            List<Contactos> lstContacto = new List<Contactos>();
                            if (Session["lstContactoEmp"] != null)
                                lstContacto = (List<Contactos>)Session["lstContactoEmp"];
                            foreach (var item in lstContacto)
                            {
                                EMPLEADO_CONTACTO oContacto = new EMPLEADO_CONTACTO();
                                oContacto.idEmpleado = eMPLEADO.id;
                                oContacto.idContactoTipo = item.idContactoTipo;
                                oContacto.valor = item.valor;
                                oContacto.observaciones = item.observaciones;
                                db.EMPLEADO_CONTACTO.Add(oContacto);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesEmpleado = "Error al guardar el Técnico, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesEmpleado = "Por favor controlar, existe algun dato erroneo.";

                }
                return View(eMPLEADO);

            }

        }

        public JsonResult AddContactoEmpleado(int idContactoTipo, string valor, string observaciones, string ContactoTipo)
        {

            Contactos oContacto = new Contactos();
            ContactoResult lstContacto = new ContactoResult();
            int idContacto = 0;

            if (Session["lstContactoEmp"] != null)
            {
                lstContacto.contactos = (List<Contactos>)Session["lstContactoEmp"];
                if (lstContacto.contactos.Count() > 0)
                    idContacto = lstContacto.contactos.LastOrDefault().idContacto;
            }

            if (lstContacto.contactos.Count > 0)
            {
                foreach (var item in lstContacto.contactos)
                {
                    if (item.idContactoTipo == idContactoTipo && item.valor.Trim() == valor.Trim())
                    {
                        lstContacto.success = 0;
                        lstContacto.mensajeError = "Contacto repetido";
                        return Json(lstContacto, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            oContacto.idContacto = idContacto + 1;
            oContacto.ContactoTipo = ContactoTipo;
            oContacto.idContactoTipo = idContactoTipo;
            oContacto.valor = valor;
            oContacto.observaciones = observaciones;
            lstContacto.contactos.Add(oContacto);
            lstContacto.success = 1;


            Session["lstContactoEmp"] = lstContacto.contactos;


            return Json(lstContacto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteContactoEmpleado(int idContacto)
        {

            Contactos oContacto = new Contactos();
            ContactoResult lstContacto = new ContactoResult();

            if (Session["lstContactoEmp"] != null)
            {
                lstContacto.contactos = (List<Contactos>)Session["lstContactoEmp"];

                lstContacto.contactos.RemoveAll(w => w.idContacto == idContacto);
                lstContacto.success = 1;
                Session["lstContactoEmp"] = lstContacto.contactos;
            }
            else
            {
                lstContacto.success = 0;
                lstContacto.mensajeError = "Error Inesperado";
            }

            return Json(lstContacto, JsonRequestBehavior.AllowGet);
        }

        // GET: EMPLEADO/Edit/5
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
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            if (eMPLEADO == null)
            {
                return HttpNotFound();
            }
            cleanSession();

            //buscar Contactos
            ContactoResult lstContacto = new ContactoResult();
            lstContacto.contactos = getContactos(eMPLEADO.id);
            Session["lstContactoEmp"] = lstContacto.contactos;
            lstContacto.success = 1;
            ViewBag.lstContacto = lstContacto;

            ViewBag.tiposContactos = new SelectList(db.CONTACTO_TIPO, "id", "descripcion");
            string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == eMPLEADO.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
            ViewBag.DocumentoTipo = nombre;            
            return View(eMPLEADO);
        }

        public List<Contactos> getContactos(int idEmpleado)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<Contactos> lstContactos = new List<Contactos>();
            Contactos oContactos = new Contactos();

            List<EMPLEADO_CONTACTO> lst = db.EMPLEADO_CONTACTO.Where(w => w.idEmpleado == idEmpleado).ToList();
            int index = 0;
            foreach (var item in lst)
            {
                index++;
                oContactos = new Contactos();
                oContactos.idContacto = index;
                oContactos.idContactoTipo = item.idContactoTipo;
                oContactos.valor = item.valor;
                oContactos.observaciones = item.observaciones;
                oContactos.ContactoTipo = db.CONTACTO_TIPO.Where(w => w.id == item.idContactoTipo).Select(s => s.descripcion).First().ToString();
                lstContactos.Add(oContactos);
            }
            return lstContactos;


        }

        // POST: EMPLEADO/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idDocumentoTipo,numeroDocumento,apellido,nombre,baja")] EMPLEADO eMPLEADO)
        {
            ViewBag.ValidacionesEmpleado = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                
                EMPLEADO emp = db.EMPLEADO.Find(eMPLEADO.id);
                               
                var comboDocumentos = db.DOCUMENTO_TIPO.ToList();
                ViewBag.idDocumentoTipo = new SelectList(comboDocumentos, "id", "descripcion", eMPLEADO.idDocumentoTipo);

                var comboTiposContactos = db.CONTACTO_TIPO.ToList();
                ViewBag.tiposContactos = new SelectList(comboTiposContactos, "id", "descripcion");

                string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == eMPLEADO.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
                ViewBag.DocumentoTipo = nombre;
                
                //buscar Contactos
                ContactoResult lstContacto = new ContactoResult();
                if (Session["lstContactoEmp"] != null)
                {
                    lstContacto.contactos = (List<Contactos>)Session["lstContactoEmp"];
                }

                lstContacto.success = 1;
                ViewBag.lstContacto = lstContacto;


                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            emp.apellido = eMPLEADO.apellido;
                            emp.nombre = eMPLEADO.nombre;


                            db.Entry(emp).State = EntityState.Modified;
                            // db.SaveChanges();
                            List<EMPLEADO_CONTACTO> contactos_borrar = db.EMPLEADO_CONTACTO.Where(w => w.idEmpleado == eMPLEADO.id).ToList();
                            db.EMPLEADO_CONTACTO.RemoveRange(contactos_borrar);
                            db.SaveChanges();
                            foreach (var item in lstContacto.contactos)
                            {
                                EMPLEADO_CONTACTO oContacto = new EMPLEADO_CONTACTO();
                                oContacto.idEmpleado = eMPLEADO.id;
                                oContacto.idContactoTipo = item.idContactoTipo;
                                oContacto.valor = item.valor;
                                oContacto.observaciones = item.observaciones;
                                db.EMPLEADO_CONTACTO.Add(oContacto);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesEmpleado = "Error al guardar el Técnico, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesEmpleado = "Por favor controlar, existe algun dato erroneo.";
                }
            }
            return View(eMPLEADO);

        }

        // GET: EMPLEADO/Delete/5
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
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            if (eMPLEADO == null)
            {
                return HttpNotFound();
            }
            return View(eMPLEADO);
        }

        // POST: EMPLEADO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            eMPLEADO.baja = true;
            db.Entry(eMPLEADO).State = EntityState.Modified;
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
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            if (eMPLEADO == null)
            {
                return HttpNotFound();
            }
            return View(eMPLEADO);
        }


        [HttpPost, ActionName("Reactivar")]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivarConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
            eMPLEADO.baja = false;
            db.Entry(eMPLEADO).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void cleanSession()
        {
            Session["lstContactoEmp"] = null;
        }
    }
}
