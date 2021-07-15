using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class GrupoViewModel
    {
        [Key]
        public int GRUP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CARGO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O CARGO deve ter no minimo 1 caractere e no máximo 50 caracteres.")]
        public string GRUP_NM_NOME { get; set; }
        public Nullable<int> GRUP_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NÙMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O CARGO deve ter no minimo 1 caractere e no máximo 10 caracteres.")]
        public string GRUP_NR_NUMERO { get; set; }
        public string GR_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CENTRO_CUSTO> CENTRO_CUSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBGRUPO> SUBGRUPO { get; set; }
    }
}