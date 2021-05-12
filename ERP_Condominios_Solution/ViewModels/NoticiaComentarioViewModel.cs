using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class NoticiaComentarioViewModel
    {
        [Key]
        public int NOCO_CD_ID { get; set; }
        public int NOTC_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> NOCO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string NOCO_DS_COMENTARIO { get; set; }
        public int NOCO_IN_ATIVO { get; set; }

        public virtual NOTICIA NOTICIA { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}