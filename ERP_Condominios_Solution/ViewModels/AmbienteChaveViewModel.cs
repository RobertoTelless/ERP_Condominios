using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AmbienteChaveViewModel
    {
        [Key]
        public int AMCH_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo AMBIENTE obrigatorio")]
        public int AMBI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE ENTREGA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENTREGA deve ser uma data válida")]
        public System.DateTime AMCH_DT_ENTREGA { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public System.DateTime AMCH_DT_PREVISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE DEVOLUÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> AMCH_DT_DEVOLUCAO { get; set; }
        [StringLength(250, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 250 caracteres.")]
        public string AMCH_DS_JUSTIFICATIVA { get; set; }
        [StringLength(1000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string AMCH_TX_OBSERVACOES { get; set; }
        public int AMCH_IN_ATIVO { get; set; }

        public virtual AMBIENTE AMBIENTE { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}