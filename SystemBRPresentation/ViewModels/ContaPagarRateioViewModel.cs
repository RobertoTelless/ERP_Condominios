using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace LeveGestao_Presentation.ViewModels
{
    public class ContaPagarRateioViewModel
    {
        [Key]
        public int CPRA_CD_ID { get; set; }
        public int CAPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CENTRO DE CUSTOS obrigatorio")]
        public int CECU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PERCENTUAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int CRPA_NR_PERCENTUAL { get; set; }
        public int CRPA_IN_ATIVO { get; set; }

        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CONTA_PAGAR CONTA_PAGAR { get; set; }
    }
}