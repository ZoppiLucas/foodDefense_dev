using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class ContactoResult
    {
        public List<Contactos> contactos { get; set; } = new List<Contactos>();
        public int success { get; set; }
        public string mensajeError { get; set; } = "";

    }
}