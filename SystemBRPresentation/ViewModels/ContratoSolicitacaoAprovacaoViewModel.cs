using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ContratoSolicitacaoAprovacaoViewModel
    {
        [Key]
        public int CTSA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CONTRATO obrigatorio")]
        public int CONT_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime CTSA_DT_DATA { get; set; }
        public int COLA_CD_ID { get; set; }
        [StringLength(5000, ErrorMessage = "A RESPOSTA deve conter no máximo 5000.")]
        public string CTSA_DS_RESPOSTA { get; set; }
        public Nullable<int> CTSA_IN_STATUS { get; set; }
        public int CTSA_IN_ATIVO { get; set; }
        public string CTSA_NM_STATUS { get; set; }
        public string CTSA_NM_APROVADOR { get; set; }

        public virtual CONTRATO CONTRATO { get; set; }

    }
}