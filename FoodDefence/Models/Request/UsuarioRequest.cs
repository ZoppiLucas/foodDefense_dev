using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Request
{
    public class UsuarioRequest
    {
        
        public string nombre { get; set; }
        public string mail { get; set; }
        public string clave { get; set; }
        public string version { get; set; }
    }
}