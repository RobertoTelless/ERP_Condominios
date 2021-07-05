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
    
    public partial class UNIDADE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UNIDADE()
        {
            this.AMBIENTE_CHAVE = new HashSet<AMBIENTE_CHAVE>();
            this.AUTORIZACAO_ACESSO = new HashSet<AUTORIZACAO_ACESSO>();
            this.CONTROLE_VEICULO = new HashSet<CONTROLE_VEICULO>();
            this.ENCOMENDA = new HashSet<ENCOMENDA>();
            this.ENTRADA_SAIDA = new HashSet<ENTRADA_SAIDA>();
            this.LISTA_CONVIDADO = new HashSet<LISTA_CONVIDADO>();
            this.OCORRENCIA = new HashSet<OCORRENCIA>();
            this.RESERVA = new HashSet<RESERVA>();
            this.SOLICITACAO_MUDANCA = new HashSet<SOLICITACAO_MUDANCA>();
            this.USUARIO = new HashSet<USUARIO>();
            this.VAGA = new HashSet<VAGA>();
            this.VEICULO = new HashSet<VEICULO>();
            this.UNIDADE_ANEXO = new HashSet<UNIDADE_ANEXO>();
            this.PRODUTO = new HashSet<PRODUTO>();
        }
    
        public int UNID_CD_ID { get; set; }
        public int TORR_CD_ID { get; set; }
        public int TIUN_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public string UNID_NR_NUMERO { get; set; }
        public Nullable<int> UNID_IN_ATIVO { get; set; }
        public Nullable<int> UNID_IN_ALUGADA { get; set; }
        public string UNID_NM_NOME_TORRE { get; set; }
        public string UNID_NM_EXIBE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_CHAVE> AMBIENTE_CHAVE { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AUTORIZACAO_ACESSO> AUTORIZACAO_ACESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTROLE_VEICULO> CONTROLE_VEICULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA> ENCOMENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENTRADA_SAIDA> ENTRADA_SAIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LISTA_CONVIDADO> LISTA_CONVIDADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA> OCORRENCIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA> RESERVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA> SOLICITACAO_MUDANCA { get; set; }
        public virtual TIPO_UNIDADE TIPO_UNIDADE { get; set; }
        public virtual TORRE TORRE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VAGA> VAGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO> VEICULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UNIDADE_ANEXO> UNIDADE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
    }
}
