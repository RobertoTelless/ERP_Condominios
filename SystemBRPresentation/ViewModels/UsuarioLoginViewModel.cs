using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class UsuarioLoginViewModel
    {
        [Key]
        public int USUA_CD_ID { get; set; }
        public int PERF_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CARG_CD_ID { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 caractere e no máximo 50.")]
        public string USUA_NM_NOME { get; set; }
        public string USUA_NM_MATRICULA { get; set; }
        [StringLength(15, ErrorMessage = "O LOGIN deve ter no máximo 15 caracteres.")]
        public string USUA_NM_LOGIN { get; set; }
        //[Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve ter no minimo 1 caractere e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string USUA_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve ter no máximo 50 caracteres.")]
        public string USUA_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve ter no máximo 50 caracteres.")]
        public string USUA_NR_CELULAR { get; set; }
        //[Required(ErrorMessage = "Campo SENHA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O SENHA deve ter no minimo 1 caractere e no máximo 10.")]
        public string USUA_NM_SENHA { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A CONFIRMAÇÃO DE SENHA deve ter no minimo 1 caractere e no máximo 10.")]
        public string USUA_NM_SENHA_CONFIRMA { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A NOVA SENHA deve ter no minimo 1 caractere e no máximo 10.")]
        public string USUA_NM_NOVA_SENHA { get; set; }
        public int USUA_IN_BLOQUEADO { get; set; }
        public int USUA_IN_PROVISORIO { get; set; }
        public int USUA_IN_LOGIN_PROVISORIO { get; set; }
        public int USUA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE BLOQUEIO Deve ser uma data válida")]
        public Nullable<System.DateTime> USUA_DT_BLOQUEADO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE ALTERAÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> USUA_DT_ALTERACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> USUA_DT_TROCA_SENHA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> USUA_DT_ACESSO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> USUA_DT_ULTIMA_FALHA { get; set; }
        public System.DateTime USUA_DT_CADASTRO { get; set; }
        public int USUA_NR_ACESSOS { get; set; }
        public int USUA_NR_FALHAS { get; set; }
        public string USUA_TX_OBSERVACOES { get; set; }
        public string USUA_AQ_FOTO { get; set; }
        public Nullable<int> USUA_IN_COMPRADOR { get; set; }
        public Nullable<int> USUA_IN_APROVADOR { get; set; }
        public Nullable<int> COLA_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public Nullable<int> FUNC_CD_ID { get; set; }
        public Nullable<int> USUA_IN_LOGADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_LOGADO { get; set; }

        public bool Bloqueio
        {
            get
            {
                if (USUA_IN_BLOQUEADO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                USUA_IN_BLOQUEADO = (value == true) ? 1 : 0;
            }
        }
        public bool LoginProvisorio
        {
            get
            {
                if (USUA_IN_LOGIN_PROVISORIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                USUA_IN_LOGIN_PROVISORIO = (value == true) ? 1 : 0;
            }
        }
        public bool Provisoria
        {
            get
            {
                if (USUA_IN_PROVISORIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                USUA_IN_PROVISORIO = (value == true) ? 1 : 0;
            }
        }

        public bool Comprador
        {
            get
            {
                if (USUA_IN_COMPRADOR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                USUA_IN_COMPRADOR = (value == true) ? 1 : 0;
            }
        }
        public bool Aprovador
        {
            get
            {
                if (USUA_IN_APROVADOR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                USUA_IN_APROVADOR = (value == true) ? 1 : 0;
            }
        }


        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CARGO CARGO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRATO_COMENTARIO> CONTRATO_COMENTARIO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FUNCIONARIO FUNCIONARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_MATERIA_PRIMA> MOVIMENTO_ESTOQUE_MATERIA_PRIMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
        public virtual PERFIL PERFIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_NOTIFICACAO> TAREFA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA1 { get; set; }
    }
}