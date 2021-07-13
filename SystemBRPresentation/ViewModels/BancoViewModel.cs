using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;

namespace SystemBRPresentation.ViewModels
{
    public class BancoViewModel
    {
        [Key]
        public int BANC_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CÓDIGO DO BANCO obrigatorio")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "O CÓDIGO DO BANCO deve conter no minimo 3 caracteres e no máximo 5.")]
        public string BANC_SG_CODIGO { get; set; }
        [Required(ErrorMessage = "Campo NOME DO BANCO obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O NOME DO BANCO deve conter no minimo 3 caracteres e no máximo 50.")]
        public string BANC_NM_NOME { get; set; }
        public int BANC_IN_ATIVO { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO> CONTA_BANCO { get; set; }
    }
}