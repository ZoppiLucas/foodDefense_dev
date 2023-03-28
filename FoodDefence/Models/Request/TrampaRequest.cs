using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Request
{
    public class TrampaRequest
    {
        //control de datos queridos
        public string is_idEstado { get; set; } = "N";
        public string is_idAccion { get; set; } = "N";
        public string is_moscas { get; set; } = "N";
        public string is_mosquitas { get; set; } = "N";
        public string is_polillas { get; set; } = "N";
        public string is_mariposas { get; set; } = "N";
        public string is_minusculos { get; set; } = "N";
        public string is_cantidad { get; set; } = "N";        
        public string is_roedor { get; set; } = "N";
        public string is_insecto { get; set; } = "N";
        public string is_cucaGermanica { get; set; } = "N";
        public string is_cucaAmericana { get; set; } = "N";

        //valores
        public int idOrdenTrabajoDetalle { get; set; }
        public string observaciones { get; set; }

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
        public int idTipoTrampa { get; set; } = 0;

    }
}