using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ControleVeiculoAcompanhamentoViewModel
    {
        [Key]
        public int CVAC_CD_ID { get; set; }
        public int COVE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CVAC_DT_ACOMPANHAMENTO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string CVAC_DS_TEXTO { get; set; }
        public Nullable<int> CVAC_IN_ATIVO { get; set; }

        public virtual CONTROLE_VEICULO CONTROLE_VEICULO { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}