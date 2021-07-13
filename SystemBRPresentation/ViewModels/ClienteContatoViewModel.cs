using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ClienteContatoViewModel
    {
        [Key]
        public int CLCO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public int CLIE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CLCO_NM_NOME { get; set; }
        [StringLength(50, ErrorMessage = "O CARGO deve conter no máximo 50.")]
        public string CLCO_NM_CARGO { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CLCO_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string CLCO_NM_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 500.")]
        public string CLCO_DS_OBSERVACOES { get; set; }
        public int CLCO_IN_ATIVO { get; set; }

        public virtual CLIENTE CLIENTE { get; set; }
    }
}