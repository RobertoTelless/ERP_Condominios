using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ReservaComentarioViewModel
    {
        [Key]
        public int RECO_CD_ID { get; set; }
        public int RESE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> RECO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string RECO_DS_COMENTARIO { get; set; }
        public Nullable<int> RECO_IN_ATIVO { get; set; }

        public virtual RESERVA RESERVA { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}