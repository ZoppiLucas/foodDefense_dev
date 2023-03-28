using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Response
{
    public class EmpListTaskResponse
    {
        public int idCliente { get; set; } = 0;
        public string razonSocial { get; set; } = "";
        public int idSector { get; set; } = 0;
        public string sector { get; set; } = "";
        public string tipoTrampa { get; set; } = "";
        public int cnt { get; set; } = 0;
    }
}