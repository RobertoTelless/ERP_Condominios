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
    
    public partial class TIPO_UNIDADE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPO_UNIDADE()
        {
            this.UNIDADE = new HashSet<UNIDADE>();
        }
    
        public int TIUN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public string TIUN_NM_NOME { get; set; }
        public Nullable<int> TIUN_IN_ATIVO { get; set; }
        public Nullable<decimal> TIUN_NR_AREA { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UNIDADE> UNIDADE { get; set; }
    }
}
