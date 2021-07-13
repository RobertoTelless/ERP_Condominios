using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class CategoriaProdutoViewModel
    {
        [Key]
        public int CAPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CAPR_NM_NOME { get; set; }
        public Nullable<int> CAPR_IN_ATIVO { get; set; }

        public int CAPR_IN_FOOD { get; set; }
        public int CAPR_IN_EXPEDICAO { get; set; }
        public int CAPR_IN_SEO { get; set; }
        public int CAPR_IN_GRADE { get; set; }
        public int CAPR_IN_TAMANHO { get; set; }

        public bool Food
        {
            get
            {
                if (CAPR_IN_FOOD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPR_IN_FOOD = value ? 1 : 0;
            }
        }
        public bool Exp
        {
            get
            {
                if (CAPR_IN_EXPEDICAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPR_IN_EXPEDICAO = value ? 1 : 0;
            }
        }
        public bool Ecom
        {
            get
            {
                if (CAPR_IN_SEO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPR_IN_SEO = value ? 1 : 0;
            }
        }
        public bool Varejo
        {
            get
            {
                if (CAPR_IN_GRADE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPR_IN_GRADE = value ? 1 : 0;
            }
        }

        public bool Grade
        {
            get
            {
                if (CAPR_IN_TAMANHO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPR_IN_TAMANHO = value ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }

    }
}