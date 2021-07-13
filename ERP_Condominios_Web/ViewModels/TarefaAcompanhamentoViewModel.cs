using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TarefaAcompanhamentoViewModel
    {
        [Key]
        public int TAAC_CD_ID { get; set; }
        public int TARE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O ACOMPANHAMENTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string TAAC_DS_ACOMPANHAMENTO { get; set; }
        public int TAAC_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime TAAC_DT_ACOMPANHAMENTO { get; set; }

        public virtual TAREFA TAREFA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}