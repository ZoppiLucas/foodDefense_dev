using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Response
{
    public class LoginResponse
    {
        public string mensaje { get; set; } = "";
        public bool success { get; set; } = false;

        public int id { get; set; } = 0;
        public string nombre { get; set; } = "";
        public string mail { get; set; } = "";
        public int? idUsuarioTipo { get; set; } = 0;
        public int? idEmpleado { get; set; } = 0;
    }
}