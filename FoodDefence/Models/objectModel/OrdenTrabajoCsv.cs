using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class OrdenTrabajoCsv
    {    
        public DateTime fechadetrabajo { get; set; }
        public string descripcion { get; set; }
        public string numero { get; set; }
        public string estado { get; set; }
        public string accion { get; set; }
        public int moscas { get; set; }
        public int mosquitas { get; set; }
        public int polillas { get; set; }
        public int mariposas { get; set; }
        public int minusculos { get; set; }
        public int roedor { get; set; }
        public int insecto { get; set; }
        public int cantidad { get; set; }
        public int cucaAmericana { get; set; }
        public int cucaGermanica { get; set; }
        public string observaciones { get; set; }
    }
}