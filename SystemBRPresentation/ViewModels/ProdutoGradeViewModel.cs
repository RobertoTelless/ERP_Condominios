using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ProdutoGradeViewModel
    {
        [Key]
        public int PRGR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo COR obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A COR deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PRGR_NM_COR { get; set; }
        public string PRGR_NM_TAMANHO { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PRGR_QN_QUANTIDADE { get; set; }
        public int PRGR_IN_ATIVO { get; set; }
        public Nullable<int> TAMA_CD_ID { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
        public virtual TAMANHO TAMANHO { get; set; }

    }
}