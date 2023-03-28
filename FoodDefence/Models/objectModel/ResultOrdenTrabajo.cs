using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class ResultOrdenTrabajo
    {
        public ORDEN_TRABAJO orden_trabajo { get; set; } = new ORDEN_TRABAJO();
        public bool success { get; set; }
        public string errMsg { get; set; }
    }
}