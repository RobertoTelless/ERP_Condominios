using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class OcorrenciaComentarioViewModel
    {
        [Key]
        public int OCCO_CD_ID { get; set; }
        public int OCOR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime OCCO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string OCCO_TX_COMENTARIO { get; set; }
        public int OCCO_IN_STATUS { get; set; }
        public System.DateTime OCCO_DT_CADASTRO { get; set; }
        public int OCCO_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }

        public virtual OCORRENCIA OCORRENCIA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}