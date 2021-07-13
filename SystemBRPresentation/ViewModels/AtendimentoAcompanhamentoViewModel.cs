using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class AtendimentoAcompanhamentoViewModel
    {
        [Key]
        public int ATAC_CD_ID { get; set; }
        public int ATEN_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ATAC_DT_ACOMPANHAMENTO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O ACOMPANHAMENTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string ATAC_DS_ACOMPANHAMENTO { get; set; }
        public int ATAC_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ATAC_DT_ENCERRAMENTO { get; set; }
        [StringLength(1500, ErrorMessage = "O ENCERRAMENTO deve conter no máximo 1500.")]
        public string ATAC_DS_ENCERRAMENTO { get; set; }

        public virtual ATENDIMENTO ATENDIMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}