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
    
    public partial class MATERIAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MATERIAL()
        {
            this.MATERIAL_ANEXO = new HashSet<MATERIAL_ANEXO>();
            this.MATERIAL_FORNECEDOR = new HashSet<MATERIAL_FORNECEDOR>();
            this.MATERIAL_MOVIMENTO = new HashSet<MATERIAL_MOVIMENTO>();
        }
    
        public int MATE_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> TIMA_CD_ID { get; set; }
        public Nullable<int> UNMA_CD_ID { get; set; }
        public string MATE_AQ_FOTO { get; set; }
        public string MATE_CD_CODIGO { get; set; }
        public string MATE_NM_NOME { get; set; }
        public string MATE_DS_DESCRICAO { get; set; }
        public Nullable<int> MATE_QN_QUANTIDADE_MINIMA { get; set; }
        public Nullable<int> MATE_QN_QUANTIDADE_MAXIMA { get; set; }
        public Nullable<int> MATE_QN_QUANTIDADE_INICIAL { get; set; }
        public Nullable<int> MATE_QN_ESTOQUE { get; set; }
        public Nullable<System.DateTime> MATE_DT_ULTIMA_MOVIMENTACAO { get; set; }
        public Nullable<int> MATE_IN_AVISA_MINIMO { get; set; }
        public Nullable<int> MATE_IN_ATIVO { get; set; }
        public Nullable<decimal> MATE_VL_ULTIMA_COMPRA { get; set; }
        public string MATE_NM_LOCALIZACAO_ESTOQUE { get; set; }
        public string MATE_TX_OBSERVACOES { get; set; }
        public string MATE_NM_MARCA { get; set; }
        public string MATE_NM_MODELO { get; set; }
        public string MATE_NM_REFERENCIA { get; set; }
        public string MATE_NR_BARCODE { get; set; }
        public string MATE_NR_QRCODE { get; set; }
        public string MATE_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> MATE_QN_CONTAGEM { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL_ANEXO> MATERIAL_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL_FORNECEDOR> MATERIAL_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL_MOVIMENTO> MATERIAL_MOVIMENTO { get; set; }
        public virtual TIPO_MATERIAL TIPO_MATERIAL { get; set; }
        public virtual UNIDADE_MATERIAL UNIDADE_MATERIAL { get; set; }
    }
}
