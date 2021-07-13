using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;

namespace SystemBRPresentation.ViewModels
{
    public class ContaBancariaLancamentoViewModel
    {
        [Key]
        public int CBLA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CONTA obrigatorio")]
        public int COBA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CBLA_DT_LANCAMENTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> CBLA_IN_TIPO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A DESCRIÇÂO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CBLA_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CBLA_VL_VALOR { get; set; }
        public Nullable<int> CBLA_IN_ATIVO { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A DESCRIÇÂO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CBLA_NR_NUMERO { get; set; }
        public Nullable<int> CBLA_IN_ORIGEM { get; set; }

        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
    }
}