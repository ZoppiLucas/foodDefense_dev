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
    public class CLIENTEController : Controller
    {


        // GET: CLIENTE
        public ActionResult Index()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            ViewBag.idDocumentoTipo = new SelectList(db.DOCUMENTO_TIPO, "id", "descripcion");
            var cLIENTE = db.CLIENTE.Include(c => c.DOCUMENTO_TIPO);
            return View(cLIENTE.ToList());
        }

        public ActionResult ListadoCliente(int pTipoDocumento = 0, long pNumeroDocumento = 0, string pRazonSocial = "", int pBaja = 0)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            var cLIENTE = db.CLIENTE.Include(c => c.DOCUMENTO_TIPO);

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
                cLIENTE = cLIENTE.Where(n => n.baja == lBaja);
            }

            if (pTipoDocumento != 0)
                cLIENTE = cLIENTE.Where(n => n.idDocumentoTipo == pTipoDocumento);

            if (pNumeroDocumento != 0)
                cLIENTE = cLIENTE.Where(n => n.numeroDocumento.ToString().Contains(pNumeroDocumento.ToString()));

            if (pRazonSocial != "")
                cLIENTE = cLIENTE.Where(n => n.razonSocial.Contains(pRazonSocial));

            return PartialView("_ListadoCliente", cLIENTE);
        }

        // GET: CLIENTE/Details/5
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
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
            cleanSession();

            //buscar Contactos
            ContactoResult lstContacto = new ContactoResult();
            lstContacto.contactos = getContactos(cLIENTE.id);
            Session["lstContacto"] = lstContacto.contactos;
            lstContacto.success = 1;
            ViewBag.lstContacto = lstContacto;                        
            string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == cLIENTE.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
            ViewBag.DocumentoTipo = nombre;
            
            return View(cLIENTE);
        }

        // GET: CLIENTE/Create
        public ActionResult Create()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            cleanSession();

            ViewBag.ValidacionesCliente = "";
            ViewBag.idDocumentoTipo = new SelectList(db.DOCUMENTO_TIPO, "id", "descripcion");
            ViewBag.tiposContactos = new SelectList(db.CONTACTO_TIPO, "id", "descripcion");
            ViewBag.lstContacto = "";
            return View();
        }

        // POST: CLIENTE/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idDocumentoTipo,numeroDocumento,razonSocial,habitlitadoPortal,usuarioPortal,clavePortal")] CLIENTE cLIENTE)
        {
            ViewBag.ValidacionesCliente = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                var comboDocumentos = db.DOCUMENTO_TIPO.ToList();
                ViewBag.idDocumentoTipo = new SelectList(comboDocumentos, "id", "descripcion", cLIENTE.idDocumentoTipo);

                var comboTiposContactos = db.CONTACTO_TIPO.ToList();
                ViewBag.tiposContactos = new SelectList(comboTiposContactos, "id", "descripcion");

                //buscar Contactos
                ContactoResult lstContactoVBag = new ContactoResult();
                if (Session["lstContacto"] != null)
                {
                    lstContactoVBag.contactos = (List<Contactos>)Session["lstContacto"];
                }

                lstContactoVBag.success = 1;
                ViewBag.lstContacto = lstContactoVBag;

                var lCliente = db.CLIENTE.Where(n => n.idDocumentoTipo == cLIENTE.idDocumentoTipo && n.numeroDocumento == cLIENTE.numeroDocumento).Count();
                if (lCliente > 0)
                {
                    ViewBag.ValidacionesCliente = "Ya existe un cliente para el documento " + db.DOCUMENTO_TIPO.Where(n => n.id == cLIENTE.idDocumentoTipo).FirstOrDefault().descripcion.ToString() + " " + cLIENTE.numeroDocumento.ToString() + ".";                    
                    return View(cLIENTE);
                }

                cLIENTE.baja = false;
                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction()) {
                        try
                        {
                            db.CLIENTE.Add(cLIENTE);
                            db.SaveChanges();

                            List<Contactos> lstContacto = new List<Contactos>();
                            if (Session["lstContacto"] != null)
                                lstContacto = (List<Contactos>)Session["lstContacto"];
                            foreach (var item in lstContacto)
                            {
                                CLIENTE_CONTACTO oContacto = new CLIENTE_CONTACTO();
                                oContacto.idCliente = cLIENTE.id;
                                oContacto.idContactoTipo = item.idContactoTipo;
                                oContacto.valor = item.valor;
                                oContacto.observaciones = item.observaciones;
                                db.CLIENTE_CONTACTO.Add(oContacto);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesCliente = "Error al guardar el cliente, por favor cerrar y volver a probar.";                            
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesCliente = "Por favor controlar, existe algun dato erroneo.";
                }
                return View(cLIENTE);
            }
        }

        public JsonResult AddContactoCliente(int idContactoTipo, string valor, string observaciones, string ContactoTipo)
        {

            Contactos oContacto = new Contactos();
            ContactoResult lstContacto = new ContactoResult();
            int idContacto = 0;

            if (Session["lstContacto"] != null)
            {
                lstContacto.contactos = (List<Contactos>)Session["lstContacto"];
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


            Session["lstContacto"] = lstContacto.contactos;


            return Json(lstContacto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteContactoCliente(int idContacto)
        {

            Contactos oContacto = new Contactos();
            ContactoResult lstContacto = new ContactoResult();

            if (Session["lstContacto"] != null)
            {
                lstContacto.contactos = (List<Contactos>)Session["lstContacto"];

                lstContacto.contactos.RemoveAll(w => w.idContacto == idContacto);
                lstContacto.success = 1;
                Session["lstContacto"] = lstContacto.contactos;
            }
            else {
                lstContacto.success = 0;
                lstContacto.mensajeError = "Error Inesperado";
            }

            return Json(lstContacto, JsonRequestBehavior.AllowGet);
        }

        // GET: CLIENTE/Edit/5
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
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
            cleanSession();

            //buscar Contactos
            ContactoResult lstContacto = new ContactoResult();
            lstContacto.contactos = getContactos(cLIENTE.id);
            Session["lstContacto"] = lstContacto.contactos;
            lstContacto.success = 1;            
            ViewBag.lstContacto = lstContacto;
            
            ViewBag.tiposContactos = new SelectList(db.CONTACTO_TIPO, "id", "descripcion");
            string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == cLIENTE.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
            ViewBag.DocumentoTipo = nombre;
            ViewBag.habitlitadoPortal = cLIENTE.habitlitadoPortal ? "S" : "N";
            return View(cLIENTE);
        }

        public List<Contactos> getContactos(int idCliente) {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            List<Contactos> lstContactos = new List<Contactos>();
            Contactos oContactos = new Contactos();
            
             List<CLIENTE_CONTACTO> lst = db.CLIENTE_CONTACTO.Where(w => w.idCliente == idCliente).ToList();
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

        // POST: CLIENTE/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idDocumentoTipo,numeroDocumento,razonSocial,habitlitadoPortal,usuarioPortal,clavePortal")] CLIENTE cLIENTE)
        {
            ViewBag.ValidacionesCliente = "";
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                //CLIENTE cli = db.CLIENTE.Where(w => w.id == cLIENTE.id).First();
                CLIENTE cli = db.CLIENTE.Find(cLIENTE.id);
                
                

                var comboDocumentos = db.DOCUMENTO_TIPO.ToList();
                ViewBag.idDocumentoTipo = new SelectList(comboDocumentos, "id", "descripcion", cLIENTE.idDocumentoTipo);

                var comboTiposContactos = db.CONTACTO_TIPO.ToList();
                ViewBag.tiposContactos = new SelectList(comboTiposContactos, "id", "descripcion");

                string nombre = db.DOCUMENTO_TIPO.Where(w => w.id == cLIENTE.idDocumentoTipo).Select(s => s.descripcion).First().ToString();
                ViewBag.DocumentoTipo = nombre;
                ViewBag.habitlitadoPortal = cLIENTE.habitlitadoPortal ? "S" : "N";

                //buscar Contactos
                ContactoResult lstContacto = new ContactoResult();
                if (Session["lstContacto"] != null)
                {
                    lstContacto.contactos = (List<Contactos>)Session["lstContacto"];                 
                }
                                
                lstContacto.success = 1;
                ViewBag.lstContacto = lstContacto;
                
                                
                if (ModelState.IsValid)
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            cli.razonSocial = cLIENTE.razonSocial;
                            cli.habitlitadoPortal = cLIENTE.habitlitadoPortal;
                            cli.usuarioPortal = cLIENTE.usuarioPortal;                            
                            cli.clavePortal = cLIENTE.clavePortal;

                            db.Entry(cli).State = EntityState.Modified;                            
                            // db.SaveChanges();
                            List<CLIENTE_CONTACTO> contactos_borrar = db.CLIENTE_CONTACTO.Where(w => w.idCliente == cLIENTE.id).ToList();
                            db.CLIENTE_CONTACTO.RemoveRange(contactos_borrar);
                            db.SaveChanges();
                            foreach (var item in lstContacto.contactos)
                            {
                                CLIENTE_CONTACTO oContacto = new CLIENTE_CONTACTO();
                                oContacto.idCliente = cLIENTE.id;
                                oContacto.idContactoTipo = item.idContactoTipo;
                                oContacto.valor = item.valor;
                                oContacto.observaciones = item.observaciones;
                                db.CLIENTE_CONTACTO.Add(oContacto);
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            ViewBag.ValidacionesCliente = "Error al guardar el cliente, por favor cerrar y volver a probar.";
                            dbTransaction.Rollback();
                        }
                    }
                }
                else
                {
                    ViewBag.ValidacionesCliente = "Por favor controlar, existe algun dato erroneo.";
                }
            }
            return View(cLIENTE);

        }

        // GET: CLIENTE/Delete/5
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
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
            return View(cLIENTE);
        }

        // POST: CLIENTE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
            cLIENTE.baja = true;
            db.Entry(cLIENTE).State = EntityState.Modified;
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
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
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
            CLIENTE cLIENTE = db.CLIENTE.Find(id);
            cLIENTE.baja = false;
            db.Entry(cLIENTE).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void cleanSession()
        {
            Session["lstContacto"] = null;
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
