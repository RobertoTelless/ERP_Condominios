using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AmbienteCustoViewModel
    {
        [Key]
        public int AMCU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo AMBIENTE obrigatorio")]
        public int AMBI_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250 caracteres.")]
        public string AMCU_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal AMCU_VL_BASE { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> AMCU_VL_REDUZIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal AMCU_VL_HORA { get; set; }
        public int AMCU_NM_ATIVO { get; set; }

        public virtual AMBIENTE AMBIENTE { get; set; }
    }
}