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
    
    public partial class AGENDA_CONDOMINIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AGENDA_CONDOMINIO()
        {
            this.AGENDA_CONDOMINIO_ANEXO = new HashSet<AGENDA_CONDOMINIO_ANEXO>();
        }
    
        public int AGCO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int TIAG_CD_ID { get; set; }
        public Nullable<int> RESE_CD_ID { get; set; }
        public Nullable<int> SOMU_CD_ID { get; set; }
        public System.DateTime AGCO_DT_DATA { get; set; }
        public System.TimeSpan AGCO_HR_HORA { get; set; }
        public Nullable<System.TimeSpan> AGCO_HR_FINAL { get; set; }
        public string AGCO_NM_TITULO { get; set; }
        public string AGCO_DS_DESCRICAO { get; set; }
        public int AGCO_IN_ATIVO { get; set; }
        public int AGCO_IN_STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_CONDOMINIO_ANEXO> AGENDA_CONDOMINIO_ANEXO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual RESERVA RESERVA { get; set; }
        public virtual SOLICITACAO_MUDANCA SOLICITACAO_MUDANCA { get; set; }
        public virtual TIPO_AGENDA TIPO_AGENDA { get; set; }
    }
}
