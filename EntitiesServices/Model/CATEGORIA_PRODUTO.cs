//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CATEGORIA_PRODUTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIA_PRODUTO()
        {
            this.PRODUTO = new HashSet<PRODUTO>();
            this.SUBCATEGORIA_PRODUTO = new HashSet<SUBCATEGORIA_PRODUTO>();
        }
    
        public int CAPR_CD_ID { get; set; }
        public string CAPR_NM_NOME { get; set; }
        public Nullable<int> CAPR_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}
