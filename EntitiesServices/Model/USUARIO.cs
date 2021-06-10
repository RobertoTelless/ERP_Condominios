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
    
    public partial class USUARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIO()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.AGENDA1 = new HashSet<AGENDA>();
            this.AGENDA_VINCULO = new HashSet<AGENDA_VINCULO>();
            this.AMBIENTE_CHAVE = new HashSet<AMBIENTE_CHAVE>();
            this.AUTORIZACAO_ACESSO = new HashSet<AUTORIZACAO_ACESSO>();
            this.CONTROLE_VEICULO = new HashSet<CONTROLE_VEICULO>();
            this.CORPO_DIRETIVO = new HashSet<CORPO_DIRETIVO>();
            this.ENCOMENDA = new HashSet<ENCOMENDA>();
            this.ENCOMENDA_COMENTARIO = new HashSet<ENCOMENDA_COMENTARIO>();
            this.ENTRADA_SAIDA = new HashSet<ENTRADA_SAIDA>();
            this.FORNECEDOR_COMENTARIO = new HashSet<FORNECEDOR_COMENTARIO>();
            this.FORNECEDOR_MENSAGEM = new HashSet<FORNECEDOR_MENSAGEM>();
            this.LISTA_CONVIDADO = new HashSet<LISTA_CONVIDADO>();
            this.LOG = new HashSet<LOG>();
            this.MATERIAL_MOVIMENTO = new HashSet<MATERIAL_MOVIMENTO>();
            this.NOTICIA_COMENTARIO = new HashSet<NOTICIA_COMENTARIO>();
            this.NOTIFICACAO = new HashSet<NOTIFICACAO>();
            this.OCORRENCIA = new HashSet<OCORRENCIA>();
            this.OCORRENCIA_COMENTARIO = new HashSet<OCORRENCIA_COMENTARIO>();
            this.RESERVA = new HashSet<RESERVA>();
            this.RESERVA_COMENTARIO = new HashSet<RESERVA_COMENTARIO>();
            this.SOLICITACAO_MUDANCA = new HashSet<SOLICITACAO_MUDANCA>();
            this.SOLICITACAO_MUDANCA_COMENTARIO = new HashSet<SOLICITACAO_MUDANCA_COMENTARIO>();
            this.SOLICITACAO_MUDANCA_MOVIMENTO = new HashSet<SOLICITACAO_MUDANCA_MOVIMENTO>();
            this.TAREFA = new HashSet<TAREFA>();
            this.TAREFA_ACOMPANHAMENTO = new HashSet<TAREFA_ACOMPANHAMENTO>();
            this.TAREFA_NOTIFICACAO = new HashSet<TAREFA_NOTIFICACAO>();
            this.TAREFA_VINCULO = new HashSet<TAREFA_VINCULO>();
            this.USUARIO_ANEXO = new HashSet<USUARIO_ANEXO>();
            this.USUARIO_CONTROLE_ENTRADA = new HashSet<USUARIO_CONTROLE_ENTRADA>();
            this.USUARIO_ESCALA_TRABALHO = new HashSet<USUARIO_ESCALA_TRABALHO>();
            this.USUARIO_FUNCIONARIO = new HashSet<USUARIO_FUNCIONARIO>();
            this.VEICULO = new HashSet<VEICULO>();
        }
    
        public int USUA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PERF_CD_ID { get; set; }
        public Nullable<int> CAUS_CD_ID { get; set; }
        public Nullable<int> CARG_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public string USUA_NM_NOME { get; set; }
        public string USUA_NM_LOGIN { get; set; }
        public string USUA_NM_EMAIL { get; set; }
        public string USUA_NR_MATRICULA { get; set; }
        public string USUA_NR_TELEFONE { get; set; }
        public string USUA_NR_CELULAR { get; set; }
        public string USUA_NR_WHATSAPP { get; set; }
        public string USUA_NM_SENHA { get; set; }
        public string USUA_NM_SENHA_CONFIRMA { get; set; }
        public string USUA_NM_NOVA_SENHA { get; set; }
        public Nullable<int> USUA_IN_BLOQUEADO { get; set; }
        public Nullable<int> USUA_IN_PROVISORIO { get; set; }
        public Nullable<int> USUA_IN_RESPONSAVEL { get; set; }
        public Nullable<int> USUA_IN_LOGIN_PROVISORIO { get; set; }
        public Nullable<int> USUA_IN_SISTEMA { get; set; }
        public Nullable<int> USUA_IN_MORADOR { get; set; }
        public Nullable<int> USUA_IN_PROPRIETARIO { get; set; }
        public Nullable<int> USUA_IN_FUNCIONARIO { get; set; }
        public Nullable<int> USUA_IN_PORTARIA { get; set; }
        public Nullable<int> USUA_IN_ATIVO { get; set; }
        public Nullable<int> USUA_IN_LOGADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_BLOQUEADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ALTERACAO { get; set; }
        public Nullable<System.DateTime> USUA_DT_TROCA_SENHA { get; set; }
        public Nullable<System.DateTime> USUA_DT_ACESSO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ULTIMA_FALHA { get; set; }
        public Nullable<System.DateTime> USUA_DT_CADASTRO { get; set; }
        public Nullable<int> USUA_NR_ACESSOS { get; set; }
        public Nullable<int> USUA_NR_FALHAS { get; set; }
        public string USUA_AQ_FOTO { get; set; }
        public string USUA_NR_CPF { get; set; }
        public string USUA_NR_RG { get; set; }
        public Nullable<System.DateTime> USUA_DT_NASCIMENTO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ENTRADA { get; set; }
        public Nullable<System.DateTime> USUA_DT_SAIDA { get; set; }
        public string USUA_DS_MOTIVO_SAIDA { get; set; }
        public string USUA_DS_JUSTIFICATIVA { get; set; }
        public string USUA_TX_OBSERVACOES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_VINCULO> AGENDA_VINCULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_CHAVE> AMBIENTE_CHAVE { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AUTORIZACAO_ACESSO> AUTORIZACAO_ACESSO { get; set; }
        public virtual CARGO CARGO { get; set; }
        public virtual CATEGORIA_USUARIO CATEGORIA_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTROLE_VEICULO> CONTROLE_VEICULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CORPO_DIRETIVO> CORPO_DIRETIVO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA> ENCOMENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA_COMENTARIO> ENCOMENDA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENTRADA_SAIDA> ENTRADA_SAIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_COMENTARIO> FORNECEDOR_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_MENSAGEM> FORNECEDOR_MENSAGEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LISTA_CONVIDADO> LISTA_CONVIDADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL_MOVIMENTO> MATERIAL_MOVIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA> OCORRENCIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA_COMENTARIO> OCORRENCIA_COMENTARIO { get; set; }
        public virtual PERFIL PERFIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA> RESERVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA_COMENTARIO> RESERVA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA> SOLICITACAO_MUDANCA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA_COMENTARIO> SOLICITACAO_MUDANCA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA_MOVIMENTO> SOLICITACAO_MUDANCA_MOVIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_NOTIFICACAO> TAREFA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_VINCULO> TAREFA_VINCULO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_CONTROLE_ENTRADA> USUARIO_CONTROLE_ENTRADA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ESCALA_TRABALHO> USUARIO_ESCALA_TRABALHO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_FUNCIONARIO> USUARIO_FUNCIONARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO> VEICULO { get; set; }
    }
}
