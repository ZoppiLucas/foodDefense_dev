using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class TipoTrampas_OrdenTrabajoDetalle
    {
        public int idTipoTrampa { get; set; }
        public string descTipoTrampa { get; set; }
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
        public  List<Trampas_OrdenTrabajoDetalle> trampas { get; set; } = new List<Trampas_OrdenTrabajoDetalle>();
    }
}