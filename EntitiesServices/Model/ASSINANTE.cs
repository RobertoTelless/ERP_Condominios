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
    
    public partial class ASSINANTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ASSINANTE()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.AGENDA_CONDOMINIO = new HashSet<AGENDA_CONDOMINIO>();
            this.AMBIENTE = new HashSet<AMBIENTE>();
            this.AMBIENTE_CHAVE = new HashSet<AMBIENTE_CHAVE>();
            this.ASSINANTE_ANEXO = new HashSet<ASSINANTE_ANEXO>();
            this.AUTORIZACAO_ACESSO = new HashSet<AUTORIZACAO_ACESSO>();
            this.CARGO = new HashSet<CARGO>();
            this.CATEGORIA_AGENDA = new HashSet<CATEGORIA_AGENDA>();
            this.CATEGORIA_NOTIFICACAO = new HashSet<CATEGORIA_NOTIFICACAO>();
            this.CATEGORIA_TELEFONE = new HashSet<CATEGORIA_TELEFONE>();
            this.CATEGORIA_USUARIO = new HashSet<CATEGORIA_USUARIO>();
            this.CONFIGURACAO = new HashSet<CONFIGURACAO>();
            this.CONTROLE_VEICULO = new HashSet<CONTROLE_VEICULO>();
            this.CORPO_DIRETIVO = new HashSet<CORPO_DIRETIVO>();
            this.ENCOMENDA = new HashSet<ENCOMENDA>();
            this.ENTRADA_SAIDA = new HashSet<ENTRADA_SAIDA>();
            this.FORNECEDOR = new HashSet<FORNECEDOR>();
            this.FUNCAO_CORPO_DIRETIVO = new HashSet<FUNCAO_CORPO_DIRETIVO>();
            this.GRUPO = new HashSet<GRUPO>();
            this.LISTA_CONVIDADO = new HashSet<LISTA_CONVIDADO>();
            this.LOG = new HashSet<LOG>();
            this.MATERIAL = new HashSet<MATERIAL>();
            this.MATERIAL_MOVIMENTO = new HashSet<MATERIAL_MOVIMENTO>();
            this.MOVIMENTO_ESTOQUE_PRODUTO = new HashSet<MOVIMENTO_ESTOQUE_PRODUTO>();
            this.NOTICIA = new HashSet<NOTICIA>();
            this.NOTIFICACAO = new HashSet<NOTIFICACAO>();
            this.OCORRENCIA = new HashSet<OCORRENCIA>();
            this.PERIODICIDADE_TAREFA = new HashSet<PERIODICIDADE_TAREFA>();
            this.PRODUTO = new HashSet<PRODUTO>();
            this.RESERVA = new HashSet<RESERVA>();
            this.SOLICITACAO_MUDANCA = new HashSet<SOLICITACAO_MUDANCA>();
            this.SUBCATEGORIA_PRODUTO = new HashSet<SUBCATEGORIA_PRODUTO>();
            this.SUBGRUPO = new HashSet<SUBGRUPO>();
            this.TAREFA = new HashSet<TAREFA>();
            this.TELEFONE = new HashSet<TELEFONE>();
            this.TEMPLATE = new HashSet<TEMPLATE>();
            this.TIPO_MATERIAL = new HashSet<TIPO_MATERIAL>();
            this.TIPO_TAREFA = new HashSet<TIPO_TAREFA>();
            this.TIPO_UNIDADE = new HashSet<TIPO_UNIDADE>();
            this.TIPO_VAGA = new HashSet<TIPO_VAGA>();
            this.TORRE = new HashSet<TORRE>();
            this.UNIDADE = new HashSet<UNIDADE>();
            this.UNIDADE_MATERIAL = new HashSet<UNIDADE_MATERIAL>();
            this.USUARIO = new HashSet<USUARIO>();
            this.VAGA = new HashSet<VAGA>();
            this.VEICULO = new HashSet<VEICULO>();
            this.BANCO = new HashSet<BANCO>();
            this.CENTRO_CUSTO = new HashSet<CENTRO_CUSTO>();
            this.CONTA_BANCO = new HashSet<CONTA_BANCO>();
            this.EQUIPAMENTO = new HashSet<EQUIPAMENTO>();
            this.CATEGORIA_EQUIPAMENTO = new HashSet<CATEGORIA_EQUIPAMENTO>();
            this.EQUIPAMENTO_MANUTENCAO = new HashSet<EQUIPAMENTO_MANUTENCAO>();
            this.CATEGORIA_PRODUTO = new HashSet<CATEGORIA_PRODUTO>();
        }
    
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }
        public Nullable<int> TIPE_CD_ID { get; set; }
        public string ASSI_NM_NOME { get; set; }
        public int ASSI_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> ASSI_DT_INICIO { get; set; }
        public Nullable<int> ASSI_IN_TIPO { get; set; }
        public Nullable<int> ASSI_IN_STATUS { get; set; }
        public string ASSI_NM_EMAIL { get; set; }
        public string ASSI_NM_RAZAO_SOCIAL { get; set; }
        public string ASSI_NR_CNPJ { get; set; }
        public string ASSI_NR_CPF { get; set; }
        public string ASSI_TX_OBSERVACOES { get; set; }
        public string ASSI_NM_ENDERECO { get; set; }
        public string ASSI_NR_NUMERO { get; set; }
        public string ASSI_NM_BAIRRO { get; set; }
        public string ASSI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string ASSI_NR_CEP { get; set; }
        public string ASSI_AQ_FOTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_CONDOMINIO> AGENDA_CONDOMINIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE> AMBIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_CHAVE> AMBIENTE_CHAVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_ANEXO> ASSINANTE_ANEXO { get; set; }
        public virtual TIPO_CONDOMINIO TIPO_CONDOMINIO { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AUTORIZACAO_ACESSO> AUTORIZACAO_ACESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARGO> CARGO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_NOTIFICACAO> CATEGORIA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_TELEFONE> CATEGORIA_TELEFONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_USUARIO> CATEGORIA_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFIGURACAO> CONFIGURACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTROLE_VEICULO> CONTROLE_VEICULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CORPO_DIRETIVO> CORPO_DIRETIVO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA> ENCOMENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENTRADA_SAIDA> ENTRADA_SAIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCAO_CORPO_DIRETIVO> FUNCAO_CORPO_DIRETIVO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO> GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LISTA_CONVIDADO> LISTA_CONVIDADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL> MATERIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL_MOVIMENTO> MATERIAL_MOVIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA> NOTICIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA> OCORRENCIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIODICIDADE_TAREFA> PERIODICIDADE_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA> RESERVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA> SOLICITACAO_MUDANCA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBGRUPO> SUBGRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TELEFONE> TELEFONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE> TEMPLATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_MATERIAL> TIPO_MATERIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_UNIDADE> TIPO_UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_VAGA> TIPO_VAGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TORRE> TORRE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UNIDADE> UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UNIDADE_MATERIAL> UNIDADE_MATERIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VAGA> VAGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO> VEICULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BANCO> BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CENTRO_CUSTO> CENTRO_CUSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO> CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPAMENTO> EQUIPAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_EQUIPAMENTO> CATEGORIA_EQUIPAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPAMENTO_MANUTENCAO> EQUIPAMENTO_MANUTENCAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_PRODUTO> CATEGORIA_PRODUTO { get; set; }
    }
}
