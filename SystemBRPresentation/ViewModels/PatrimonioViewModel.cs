using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class PatrimonioViewModel
    {
        [Key]
        public int PATR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 20.")]
        public string PATR_NR_NUMERO_PATRIMONIO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PATR_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "O DESCRIÇÂO deve conter no máximo 500.")]
        public string PATR_DS_DESCRICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PATR_DT_COMPRA { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PATR_VL_VALOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PATR_NR_VIDA_UTIL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PATR_DT_BAIXA { get; set; }
        [StringLength(250, ErrorMessage = "O MOTIVO DA BAIXA deve conter no máximo 250.")]
        public string PATR_DS_MOTIVO_BAIXA { get; set; }
        public int PATR_IN_ATIVO { get; set; }
        public System.DateTime PATR_DT_CADASTRO { get; set; }
        public string PATR_AQ_FOTO { get; set; }
        public string PATR_TX_OBSERVACOES { get; set; }

        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> Valor
        {
            get
            {
                return PATR_VL_VALOR;
            }
            set
            {
                PATR_VL_VALOR = value;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PATRIMONIO CATEGORIA_PATRIMONIO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PATRIMONIO_ANEXO> PATRIMONIO_ANEXO { get; set; }
    }
}