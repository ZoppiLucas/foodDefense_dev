//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FoodDefence.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA
    {
        public int id { get; set; }
        public Nullable<int> idOrdenTrabajoDetalle { get; set; }
        public Nullable<int> idTarea { get; set; }
        public Nullable<bool> realizado { get; set; }
    
        public virtual ORDEN_TRABAJO_DETALLE ORDEN_TRABAJO_DETALLE { get; set; }
        public virtual TAREA TAREA { get; set; }
    }
}
