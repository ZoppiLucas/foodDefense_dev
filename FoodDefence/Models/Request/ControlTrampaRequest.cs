using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Request
{
    public class ControlTrampaRequest
    {        
        //valores
        public int idOrdenTrabajoDetalle { get; set; }        
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
        public string observaciones { get; set; }
    }
}