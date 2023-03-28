using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.objectModel
{
    public class Trampas_OrdenTrabajo : IEquatable<Trampas_OrdenTrabajo>
    {
        public int idOrdenTrabajoDetalle { get; set; }
        public int idTrampaClienteLocacionSector { get; set; }
        public string numero { get; set; }
        public string descripcion { get; set; }
        public string nueva { get; set; }
        public int idTipoTrampa { get; set; }
        public bool cargado { get; set; }
        public string descTipoTrampa { get; set; }
        public int orden { get; set; }
        public bool Equals(Trampas_OrdenTrabajo other)
        {
            if (other is null)
                return false;

            return this.numero == other.numero;
        }

        public override bool Equals(object obj) => Equals(obj as Trampas_OrdenTrabajo);
        public override int GetHashCode() => (numero).GetHashCode();

    }
}