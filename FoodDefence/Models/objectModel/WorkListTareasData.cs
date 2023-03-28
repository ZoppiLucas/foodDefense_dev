using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class WorkListTareasInsert
    {
        public int idOrdenTrabajoDetalle { get; set; }
        public List<WorkListTareasData> listAcciones { get; set; }
    }
}