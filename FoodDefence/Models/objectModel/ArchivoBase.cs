using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class ArchivoBase
    {
        public int version { get; set; }

        public string usuario { get; set; }

        public string password { get; set; }

        public int cantRegistros { get; set; }
        public string datos { get; set; }

    }
}