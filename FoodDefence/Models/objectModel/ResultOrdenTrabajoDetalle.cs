using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class ResultOrdenTrabajoDetalle
    {
        public ORDEN_TRABAJO_DETALLE orden_trabajo_detalle { get; set; } = new ORDEN_TRABAJO_DETALLE();
        public bool success { get; set; }
        public string errMsg { get; set; }
        
    }
}