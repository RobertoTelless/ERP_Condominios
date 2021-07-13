using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class FichaTecnicaViewModel
    {
        [Key]
        public int FITE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string FITE_NM_NOME { get; set; }
        public string FITE_NM_UNITARIO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string FITE_S_DESCRICAO { get; set; }
        public System.DateTime FITE_DT_CADASTRO { get; set; }
        public int FITE_IN_ATIVO { get; set; }
        [StringLength(5000, ErrorMessage = "A APRESENTAÇÃO deve conter no máximo 5000.")]
        public string FITE_DS_APRESENTACAO { get; set; }
        public string FITE_AQ_APRESENTACAO { get; set; }

        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }

    }
}