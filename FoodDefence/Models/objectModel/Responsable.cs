using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class Responsable
    {
        //public int index { get; set; }
        
        public string nombre { get; set; }
        public string apellido { get; set; }
        public bool habilitadoPortal { get; set; } = true;
        public string usuarioPortal { get; set; }
        public string clavePortal { get; set; }
        public List<Contactos> contactos { get; set; } = new List<Contactos>();
        public bool isEdit { get; set; } = false;
        public int isEdit_Index { get; set; }
    }
}