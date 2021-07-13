using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class EquipamentoManutencaoViewModel
    {
        [Key]
        public int EQMA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int EQUI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> EQMA_DT_MANUTENCAO { get; set; }
        [Required(ErrorMessage = "Campo FORNECEDOR obrigatorio")]
        public Nullable<int> FORN_CD_ID { get; set; }
        public string EQMA_TX_OBSERVACOES { get; set; }
        public int EQMA_IN_ATIVO { get; set; }

        public virtual EQUIPAMENTO EQUIPAMENTO { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }

    }
}