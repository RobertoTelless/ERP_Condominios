using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EncomendaComentarioViewModel
    {
        [Key]
        public int ECOM_CD_ID { get; set; }
        public Nullable<int> ENCO_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ECOM_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string ECOM_DS_COMENTARIO { get; set; }
        public Nullable<int> ECOM_IN_ATIVO { get; set; }

        public virtual ENCOMENDA ENCOMENDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}