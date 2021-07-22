using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConvidadoViewModel
    {
        [Key]
        public int CONV_CD_ID { get; set; }
        public int LICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CONV_NM_CONVIDADO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CHEGADA Deve ser uma data válida")]
        public Nullable<System.DateTime> CONV_DT_CHEGADA { get; set; }
        public int CONV_IN_CHEGOU { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime CONV_DT_CADASTRO { get; set; }
        public int CONV_IN_ATIVO { get; set; }

        public virtual LISTA_CONVIDADO LISTA_CONVIDADO { get; set; }
    }
}