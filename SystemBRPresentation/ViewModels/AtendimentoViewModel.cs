using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class AtendimentoViewModel
    {
        [Key]
        public int ATEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CAAT_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        public Nullable<int> PEVE_CD_ID { get; set; }
        public Nullable<int> DEPT_CD_ID { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 5000.")]
        public string ATEN_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIAL obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime ATEN_DT_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ATEN_DT_ENCERRAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 5000.")]
        public string ATEN_DS_ENCERRAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ATEN_DT_CANCELAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 5000.")]
        public string ATEN_DS_CANCELAMENTO { get; set; }
        public int ATEN_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int ATEN_IN_STATUS { get; set; }
        public string ATEN_TX_OBSERVACOES { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> ATEN_HR_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> ATEN_HR_CANCELAMENTO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> ATEN_HR_ENCERRAMENTO { get; set; }
        [StringLength(500, ErrorMessage = "O ASSUNTO deve conter no máximo 500.")]
        public string ATEN_NM_ASSUNTO { get; set; }
        public Nullable<int> ATEN_IN_PRIORIDADE { get; set; }
        public Nullable<int> ATEN_IN_TIPO { get; set; }
        public Nullable<int> ATEN_IN_DESTINO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatório")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ATEN_DT_PREVISTA { get; set; }
        public Nullable<Int32> SERV_CD_ID { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_ACOMPANHAMENTO> ATENDIMENTO_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_ANEXO> ATENDIMENTO_ANEXO { get; set; }
        public virtual CATEGORIA_ATENDIMENTO CATEGORIA_ATENDIMENTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual DEPARTAMENTO DEPARTAMENTO { get; set; }
        public virtual PEDIDO_VENDA PEDIDO_VENDA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual SERVICO SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}