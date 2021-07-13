using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class MateriaPrimaFornecedorViewModel
    {
        [Key]
        public int MAFO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo INSUMO obrigatorio")]
        public Nullable<int> MAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORNECEDOR obrigatorio")]
        public Nullable<int> FORN_CD_ID { get; set; }
        public int MAFO_IN_ATIVO { get; set; }

        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual MATERIA_PRIMA MATERIA_PRIMA { get; set; }

    }
}