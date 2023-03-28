using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Response
{
    public class ValidacionTrampaResponse
    {
        public int cantEncontradas { get; set; }
        public List<ValidacionTrampa> validacionTrampas { get; set; } = new List<ValidacionTrampa>();
    }
}