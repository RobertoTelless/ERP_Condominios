using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ListaConvidadoComentarioViewModel
    {
        [Key]
        public int LCCM_CD_ID { get; set; }
        public int LICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime LCCM_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string LCCM_DS_COMENTARIO { get; set; }
        public int LCCM_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }

        public virtual LISTA_CONVIDADO LISTA_CONVIDADO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}