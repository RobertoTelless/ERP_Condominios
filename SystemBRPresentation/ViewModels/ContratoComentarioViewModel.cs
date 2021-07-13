using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ContratoComentarioViewModel
    {
        [Key]
        public int COCO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CONTRATO obrigatorio")]
        public int CONT_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COCO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string COCO_DS_COMENTARIO { get; set; }
        public int COCO_IN_ATIVO { get; set; }

        public virtual CONTRATO CONTRATO { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}