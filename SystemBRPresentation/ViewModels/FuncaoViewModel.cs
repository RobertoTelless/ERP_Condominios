using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class FuncaoViewModel
    {
        [Key]
        public int FNCA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CARGO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O CARGO deve ter no minimo 1 caractere e no máximo 50.")]
        public string FNCA_NM_NOME { get; set; }
        public Nullable<int> FNCA_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO> FUNCIONARIO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}