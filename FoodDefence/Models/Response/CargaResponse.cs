using FoodDefence.Models.objectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Response
{
    public class CargaResponse
    {
        public int idOrdenTrabajo { get; set; }
        public int idOrdenTrabajoDetalle { get; set; }
        public int idTrampaClienteLocacionSector { get; set; }        
        public string mensaje { get; set; } = "";
        public bool success { get; set; } = false;
        public string workDate { get; set; }
        public string workDateTime { get; set; }
        public List<string> lastTrap { get; set; } = new List<string>();
        public string workCompleted { get; set; }
        public bool mostrarAcciones { get; set; } = false;
        public List<WorkListTareasData> lstTareas { get; set; } = new List<WorkListTareasData> ();
    }
}