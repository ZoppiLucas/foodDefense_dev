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
    
    public partial class ARCHIVOS_CABECERA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ARCHIVOS_CABECERA()
        {
            this.ARCHIVOS_DETALLE = new HashSet<ARCHIVOS_DETALLE>();
        }
    
        public int id { get; set; }
        public Nullable<int> arcVersion { get; set; }
        public Nullable<System.DateTime> fechaCarga { get; set; }
        public Nullable<int> cantRegistros { get; set; }
        public Nullable<System.DateTime> fechaProceso { get; set; }
        public Nullable<int> estado { get; set; }
        public string obs { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVOS_DETALLE> ARCHIVOS_DETALLE { get; set; }
    }
}