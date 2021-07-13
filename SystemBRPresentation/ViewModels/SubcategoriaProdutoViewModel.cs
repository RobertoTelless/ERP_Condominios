using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class SubCategoriaProdutoViewModel
    {
        [Key]
        public int SCPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CARGO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O CARGO deve ter no minimo 1 caractere e no máximo 50.")]
        public string SCPR_NM_NOME { get; set; }
        public Nullable<int> SCPR_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }

    }
}