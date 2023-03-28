using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodDefence.Models;

namespace FoodDefense.Controllers
{
    public class IngresarController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        // GET: Ingresar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Ingreso()
        {
            Session["Menu"] = null;
            Session["idUsuario"] = 0;
            Session["idUsuarioTipo"] = 0;
            Session["idCliente"] = 0;
            ViewBag.Ingreso = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ingreso([Bind(Include = "id,nombre,clave,mail")] USUARIO pUSUARIO)
        {
            if (ModelState.IsValid)
            {
                if (db.USUARIO.Where(n => n.mail == pUSUARIO.mail && n.nombre == pUSUARIO.nombre && n.clave == pUSUARIO.clave).FirstOrDefault() != null)
                {
                    ViewBag.Ingreso = "";
                    Session["idUsuario"] = db.USUARIO.Where(n => n.mail.ToUpper() == pUSUARIO.mail.ToUpper() && n.nombre.ToUpper() == pUSUARIO.nombre.ToUpper() && n.clave == pUSUARIO.clave).FirstOrDefault().id;
                    Session["idUsuarioTipo"] = db.USUARIO.Where(n => n.mail.ToUpper() == pUSUARIO.mail.ToUpper() && n.nombre.ToUpper() == pUSUARIO.nombre.ToUpper() && n.clave == pUSUARIO.clave).FirstOrDefault().idUsuarioTipo;
                    Session["idCliente"] = db.USUARIO.Where(n => n.mail.ToUpper() == pUSUARIO.mail.ToUpper() && n.nombre.ToUpper() == pUSUARIO.nombre.ToUpper() && n.clave == pUSUARIO.clave).FirstOrDefault().idCliente;
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Ingreso = "No";
            return View();
        }
    }
}