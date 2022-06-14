using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ReservaViewModel
    {
        [Key]
        public int RESE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FINALIDADE obrigatorio")]
        public int FIRE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo AMBIENTE obrigatorio")]
        public int AMBI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> RECE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, ErrorMessage = "O NOME deve conter no máximo 50 caracteres.")]
        public string RESE_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA DO EVENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DO EVENTO Deve ser uma data válida")]
        public System.DateTime RESE_DT_EVENTO { get; set; }
        [Required(ErrorMessage = "Campo HORA INICIO obrigatorio")]
        public string RESE_HR_INICIO { get; set; }
        [Required(ErrorMessage = "Campo HOTA FINAL obrigatorio")]
        public string RESE_HR_FINAL { get; set; }
        public Nullable<System.DateTime> RESE_DT_INICIO { get; set; }
        public Nullable<System.DateTime> RESE_DT_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> RESE_NR_CONVIDADOS { get; set; }
        public int RESE_IN_BOLETO { get; set; }
        public string RESE_NR_BOLETO { get; set; }
        public System.DateTime RESE_DT_CADASTRO { get; set; }
        public string RESE_TX_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DA APROVAÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> RESE_DT_APROVACAO { get; set; }
        public Nullable<int> RESE_IN_ACEITA { get; set; }
        public int RESE_IN_CONFIRMADA { get; set; }
        public Nullable<int> RESE_IN_PAGA { get; set; }
        public int RESE_IN_STATUS { get; set; }
        public int RESE_IN_LEMBRAR { get; set; }
        public int RESE_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DO VETO Deve ser uma data válida")]
        public Nullable<System.DateTime> RESE_DT_VETADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DA CONFIRMAÇÂO Deve ser uma data válida")]
        public Nullable<System.DateTime> RESE_DT_CONFIRMACAO { get; set; }
        public Nullable<System.TimeSpan> RESE_HR_INICIA { get; set; }
        public Nullable<System.TimeSpan> RESE_HR_TERMINA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_CONDOMINIO> AGENDA_CONDOMINIO { get; set; }
        public virtual AMBIENTE AMBIENTE { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CONTA_RECEBER CONTA_RECEBER { get; set; }
        public virtual FINALIDADE_RESERVA FINALIDADE_RESERVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LISTA_CONVIDADO> LISTA_CONVIDADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA_ANEXO> RESERVA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA_COMENTARIO> RESERVA_COMENTARIO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}