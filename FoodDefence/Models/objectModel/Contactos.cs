using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class Contactos
    {
        public int idContacto { get; set; }
        public int idContactoTipo { get; set; }
        public string ContactoTipo { get; set; }
        public string valor { get; set; }
        public string observaciones { get; set; }

    }
}