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
    
    public partial class PRODUTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUTO()
        {
            this.MOVIMENTO_ESTOQUE_PRODUTO = new HashSet<MOVIMENTO_ESTOQUE_PRODUTO>();
            this.PRODUTO_ANEXO = new HashSet<PRODUTO_ANEXO>();
            this.PRODUTO_FORNECEDOR = new HashSet<PRODUTO_FORNECEDOR>();
        }
    
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CAPR_CD_ID { get; set; }
        public Nullable<int> SCPR_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> UNMA_CD_ID { get; set; }
        public string PROD_NM_NOME { get; set; }
        public string PROD_DS_DESCRICAO { get; set; }
        public int PROD_QN_QUANTIDADE_MINIMA { get; set; }
        public int PROD_QN_QUANTIDADE_INICIAL { get; set; }
        public int PROD_QN_ESTOQUE { get; set; }
        public Nullable<System.DateTime> PROD_DT_ULTIMA_MOVIMENTACAO { get; set; }
        public int PROD_IN_AVISA_MINIMO { get; set; }
        public System.DateTime PROD_DT_CADASTRO { get; set; }
        public int PROD_IN_ATIVO { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        public string PROD_CD_CODIGO { get; set; }
        public string PROD_DS_INFORMACOES { get; set; }
        public Nullable<int> PROD_QN_QUANTIDADE_MAXIMA { get; set; }
        public Nullable<int> PROD_QN_RESERVA_ESTOQUE { get; set; }
        public string PROD_NR_REFERENCIA { get; set; }
        public string PROD_NM_LOCALIZACAO_ESTOQUE { get; set; }
        public Nullable<decimal> PROD_VL_CUSTO { get; set; }
        public string PROD_TX_OBSERVACOES { get; set; }
        public string PROD_NM_MARCA { get; set; }
        public string PROD_NM_MODELO { get; set; }
        public string PROD_NM_REFERENCIA_FABRICANTE { get; set; }
        public string PROD_NM_FABRICANTE { get; set; }
        public string PROD_NR_BARCODE { get; set; }
        public string PROD_QR_QRCODE { get; set; }
        public string PROD_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> PROD_QN_NOVA_CONTAGEM { get; set; }
        public Nullable<int> PROD_QN_CONTAGEM { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        public virtual SUBCATEGORIA_PRODUTO SUBCATEGORIA_PRODUTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual UNIDADE_MATERIAL UNIDADE_MATERIAL { get; set; }
    }
}
