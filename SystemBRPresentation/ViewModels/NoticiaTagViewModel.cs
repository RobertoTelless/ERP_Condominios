using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBR_Presentation.ViewModels
{
    public class NoticiaTagViewModel
    {
        public int NOTA_CD_ID { get; set; }
        public int NOTC_CD_ID { get; set; }
        public int TITA_CD_ID { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME DA TAG deve ter no minimo 1 caractere e no máximo 50.")]
        public string NOTA_NM_NOME_TAG { get; set; }
        public int NOTA_IN_ATIVO { get; set; }

        public virtual NOTICIA NOTICIA { get; set; }
        public virtual TIPO_TAG TIPO_TAG { get; set; }

    }
}