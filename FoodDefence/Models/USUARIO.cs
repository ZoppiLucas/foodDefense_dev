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
    
    public partial class USUARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIO()
        {
            this.MENU_USUARIO = new HashSet<MENU_USUARIO>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public string clave { get; set; }
        public string mail { get; set; }
        public Nullable<int> idUsuarioTipo { get; set; }
        public Nullable<int> idEmpleado { get; set; }
        public Nullable<int> idCliente { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENU_USUARIO> MENU_USUARIO { get; set; }
        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual USUARIO_TIPO USUARIO_TIPO { get; set; }
    }
}