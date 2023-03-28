using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class OrdenTrabajo
    {
        //public int index { get; set; }

        public int id { get; set; }
        public string cliente { get; set; }
        public string locacion { get; set; }
        public DateTime fechaCarga { get; set; }
        public DateTime fechaTrabajo { get; set; }
        public int idOrdenTrabajoEstado { get; set; }
        public string OrdenTrabajoEstado { get; set; }
        public int cantTrampas { get; set; }

    }
}