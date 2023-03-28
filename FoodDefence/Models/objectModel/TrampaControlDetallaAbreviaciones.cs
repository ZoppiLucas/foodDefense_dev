using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class TrampaControlDetallaAbreviaciones
    {

        public int idTrampaTipo { get; set; }
        public string trampaTipoDesc { get; set; }
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
        public byte orden { get; set; }
        public string tipo { get; set; } /*E estado, A accion*/
    }
}