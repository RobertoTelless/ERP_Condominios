using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EntradaSaidaComentarioViewModel
    {
        [Key]
        public int ESCO_CD_ID { get; set; }
        public int ENSA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ESCO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string ESCO_DS_COMENTARIO { get; set; }
        public Nullable<int> ESCO_IN_ATIVO { get; set; }

        public virtual ENTRADA_SAIDA ENTRADA_SAIDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}