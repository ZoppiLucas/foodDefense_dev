using FoodDefence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDefense.Controllers
{
    public class HomeController : Controller
    {
        private FoodDefense_DevEntities db = new FoodDefense_DevEntities();

        public ActionResult Index()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            var lID = Session["idUsuario"]==null? 1 : Convert.ToInt32(Session["idUsuario"].ToString());

            var lMenu = (from u in db.USUARIO
                         join mu in db.MENU_USUARIO on u.id equals mu.idUsuario
                         join m in db.MENU on mu.idMenu equals m.id
                         where u.id == lID
                             && m.id != m.padre
                         select mu).ToList();

            Session["Menu"] = lMenu;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}