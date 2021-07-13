using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class OrdemServicoViewModel
    {
        [Key]
        public int ORSE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ATENDIMENTO obrigatorio")]
        public int ATEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime ORSE_DT_CRIACAO { get; set; }
        public string ORSE_TX_INFORMACOES { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSE_DT_PREVISTA { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        public string ORSE_NR_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "A NOTA FISCAL deve conter no máximo 20.")]
        public string ORSE_NR_NOTA_FISCAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSE_DT_CANCELAMENTO { get; set; }
        [StringLength(1000, ErrorMessage = "MOTIVO DE CANCELAMENTO deve conter no máximo 1000.")]
        public string ORSE_DS_MOTIVO_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSE_DT_ENCERRAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "O ENCERRAMENTO deve conter no máximo 5000.")]
        public string ORSE_DS_ENCERRAMENTO { get; set; }
        public int ORSE_IN_VISITA { get; set; }
        public int ORSE_IN_ATIVO { get; set; }
        public int ORSE_IN_STATUS { get; set; }
        public string ORSE_TX_OBSERVACOES { get; set; }

        public virtual ATENDIMENTO ATENDIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_ACOMPANHAMENTO> ORDEM_SERVICO_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_ANEXO> ORDEM_SERVICO_ANEXO { get; set; }
    }
}