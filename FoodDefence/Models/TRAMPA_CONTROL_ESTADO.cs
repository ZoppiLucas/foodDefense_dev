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
    
    public partial class TRAMPA_CONTROL_ESTADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRAMPA_CONTROL_ESTADO()
        {
            this.ORDEN_TRABAJO_DETALLE_ESTADO = new HashSet<ORDEN_TRABAJO_DETALLE_ESTADO>();
            this.ORDEN_TRABAJO_DETALLE = new HashSet<ORDEN_TRABAJO_DETALLE>();
        }
    
        public int id { get; set; }
        public int idTrampaTipo { get; set; }
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
        public Nullable<byte> orden { get; set; }
    
        public virtual TRAMPA_TIPO TRAMPA_TIPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEN_TRABAJO_DETALLE_ESTADO> ORDEN_TRABAJO_DETALLE_ESTADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEN_TRABAJO_DETALLE> ORDEN_TRABAJO_DETALLE { get; set; }
    }
}
