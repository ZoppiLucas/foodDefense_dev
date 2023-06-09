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
    
    public partial class ORDEN_TRABAJO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ORDEN_TRABAJO()
        {
            this.ORDEN_TRABAJO_DETALLE = new HashSet<ORDEN_TRABAJO_DETALLE>();
        }
    
        public int id { get; set; }
        public int idOrdenTrabajoEstado { get; set; }
        public int idClienteLocacion { get; set; }
        public System.DateTime fechaCargaOrden { get; set; }
        public System.DateTime fechaDeTrabajo { get; set; }
        public Nullable<int> idEmpleado { get; set; }
    
        public virtual CLIENTE_LOCACION CLIENTE_LOCACION { get; set; }
        public virtual ORDEN_TRABAJO_ESTADO ORDEN_TRABAJO_ESTADO { get; set; }
        public virtual EMPLEADO EMPLEADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEN_TRABAJO_DETALLE> ORDEN_TRABAJO_DETALLE { get; set; }
    }
}
