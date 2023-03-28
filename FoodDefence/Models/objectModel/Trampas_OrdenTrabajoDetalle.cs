using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDefence.Models.objectModel
{
    public class Trampas_OrdenTrabajoDetalle
    {
        public int idOrdenTrabajoDetalle { get; set; }
        public int idOrdenTrabajo { get; set; }
        public int idTrampaClienteLocacionSector { get; set; }
        public string numero { get; set; }
        public string descripcion { get; set; }
        public string descripcionCompleta { get; set; }
        public int idTipoTrampa { get; set; }
        public bool cargado { get; set; } = false;
        public string descTipoTrampa { get; set; }
        public int? idEstado { get; set; }
        public int? idAccion { get; set; }
        public int? moscas { get; set; } = 0;
        public int? mosquitas { get; set; } = 0;
        public int? polillas { get; set; } = 0;
        public int? mariposas { get; set; } = 0;
        public int? minusculos { get; set; } = 0;
        public int? roedor { get; set; } = 0;
        public int? insecto { get; set; } = 0;
        public int? cucaGermanica { get; set; } = 0;
        public int? cucaAmericana { get; set; } = 0;
        public int? cantidad { get; set; } = 0;
        public string is_idEstado { get; set; } = "N";
        public string is_idAccion { get; set; } = "N";
        public string is_moscas { get; set; } = "N";
        public string is_mosquitas { get; set; } = "N";
        public string is_polillas { get; set; } = "N";
        public string is_mariposas { get; set; } = "N";
        public string is_minusculos { get; set; } = "N";
        public string is_cantidad { get; set; } = "N";
        public string is_cargado { get; set; } = "N";
        public string is_roedor { get; set; } = "N";
        public string is_insecto { get; set; } = "N";
        public string is_cucaGermanica { get; set; } = "N";
        public string is_cucaAmericana { get; set; } = "N";
        public string observaciones { get; set; }
        public string descripcionEstado { get; set; }
        public string descripcionAccion { get; set; }
        public string descripcionEstadoAbr { get; set; }
        public string descripcionAccionAbr { get; set; }
        public int ordenNumero { get; set; }
        public string ordenSubNumero { get; set; }        
        public bool? multiSelTipoTrampa { get; set; } = false;

        public SelectList cmbEstado { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        public SelectList cmbAccion { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public List<Select> selectEstado { get; set; } = new List<Select>();
        public List<Select> selectAccion { get; set; } = new List<Select>();

        public List<int> listEstado { get; set; } = new List<int>();

    }
}