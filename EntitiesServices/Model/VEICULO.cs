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
    
    public partial class VEICULO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VEICULO()
        {
            this.VEICULO_ANEXO = new HashSet<VEICULO_ANEXO>();
        }
    
        public int VEIC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int UNID_CD_ID { get; set; }
        public int TIVE_CD_ID { get; set; }
        public Nullable<int> VAGA_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string VEIC_NM_PLACA { get; set; }
        public string VEIC_NM_MARCA { get; set; }
        public string VEIC_DS_DESCRICAO { get; set; }
        public string VEIC_NM_COR { get; set; }
        public string VEIC_NR_ANO { get; set; }
        public string VEIC_DS_OBSERVACOES { get; set; }
        public string VEIC_DS_JUSTIFICATIVA { get; set; }
        public System.DateTime VEIC_DT_CADASTRO { get; set; }
        public int VEIC_IN_ATIVO { get; set; }
        public Nullable<int> VEIC_IN_CONFIRMA_VAGA { get; set; }
        public Nullable<System.DateTime> VEIC_DT_CONFIRMACAO { get; set; }
        public string VEIC_AQ_FOTO { get; set; }
        public string VEIC_NM_EXIBE { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual TIPO_VEICULO TIPO_VEICULO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual VAGA VAGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO_ANEXO> VEICULO_ANEXO { get; set; }
    }
}
