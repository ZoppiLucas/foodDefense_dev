using FoodDefence.Models;
using FoodDefence.Models.objectModel;
using FoodDefence.Models.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDefence.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Chart()
        {
            if (Session["idUsuario"] == null)
                return RedirectToAction("Ingreso", "Ingresar");
            if (Convert.ToInt32(Session["idUsuario"]) == 0)
                return RedirectToAction("Ingreso", "Ingresar");

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            if (Session["idCliente"] != null)
            {
                int lIdCliente = (int)Session["idCliente"];

                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false && n.id == lIdCliente), "id", "razonSocial", null);
            }
            else
            {
                ViewBag.idCliente = new SelectList(db.CLIENTE.Where(n => n.baja == false), "id", "razonSocial", null);
            }
            GenericoRepository oGen = new GenericoRepository();
            List<Periodo> listP = oGen.getPeriodosOrdenTrabajo();

            ViewBag.cmbPeriodo = new SelectList(listP, "key", "nombre", null);
            ViewBag.lstGraficos = new SelectList(oGen.getGraficos(), "id", "descripcion", null);
                

            limpiarSession();

            return View();
            
        }

        private void limpiarSession()
        {
            Session["pGraficos"] = null;
            Session["pPeriodo"] = null;
            Session["pPeriodoText"] = null;
            Session["pClienteLocacion"] = null;
            Session["pCliente"] = null;
            
        }

        public JsonResult NewChart(List<string> pGraficos, string pPeriodo, string pPeriodoText, string pClienteLocacion, string pCliente)
        {
            Session["pGraficos"] = String.Join(",", pGraficos.ToArray()); ;
            Session["pPeriodo"] = pPeriodo;
            Session["pPeriodoText"] = pPeriodoText;
            Session["pClienteLocacion"] = pClienteLocacion;
            Session["pCliente"] = pCliente;

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult imprimirChart()
        {
            GenericoRepository oRep = new GenericoRepository();
            
            string pGraficos = Session["pGraficos"].ToString();
            string pPeriodo = Session["pPeriodo"].ToString();
            string pPeriodoText = Session["pPeriodoText"].ToString();
            int pClienteLocacion = Convert.ToInt32(Session["pClienteLocacion"]);
            int pCliente = Convert.ToInt32(Session["pCliente"]);

            byte[] byteArray = oRep.generarChart(pGraficos, pPeriodo, pPeriodoText, pClienteLocacion, pCliente);
            MemoryStream ms = new MemoryStream(byteArray);
            return new FileStreamResult(ms, "application/pdf");
        }

    }
}