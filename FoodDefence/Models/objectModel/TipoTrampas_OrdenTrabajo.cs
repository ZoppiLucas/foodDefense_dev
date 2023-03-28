using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class TipoTrampas_OrdenTrabajo
    {
        public int idTipoTrampa { get; set; }
        public string descTipoTrampa { get; set; }
      public  List<Trampas_OrdenTrabajo> trampas { get; set; } = new List<Trampas_OrdenTrabajo>();
    }
}