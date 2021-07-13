using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ClienteTagViewModel
    {
        [Key]
        public int CLTA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public int CLIE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TAG obrigatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "A TAG deve conter no minimo 1 caracteres e no máximo 20.")]
        public string CLTA_NM_TAG { get; set; }
        public Nullable<int> CLTA_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> CLTA_IN_TIPO { get; set; }

        public virtual CLIENTE CLIENTE { get; set; }

    }
}