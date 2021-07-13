using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class ValorComissaoViewModel
    {
        [Key]
        public int VACO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINATURA obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo MATRIZ obrigatorio")]
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE COMISSÃO obrigatorio")]
        public Nullable<int> TICO_CD_ID { get; set; }
        public Nullable<int> CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo VALOR DA COMISSÃO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal VACO_VL_VALOR { get; set; }
        public int VACO_IN_ATIVO { get; set; }
        public string VACO_NM_NOME { get; set; }
        public decimal ValorComissao
        {
            get
            {
                return VACO_VL_VALOR;
            }
            set
            {
                VACO_VL_VALOR = value;
            }
        }

        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMISSAO_VENDA> COMISSAO_VENDA { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        public virtual TIPO_COMISSAO TIPO_COMISSAO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARGO> CARGO { get; set; }
    }
}