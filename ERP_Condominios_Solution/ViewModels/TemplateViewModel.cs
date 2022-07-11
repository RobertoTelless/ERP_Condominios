using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TemplateViewModel
    {
        [Key]
        public int TEMP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string TEMP_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10.")]
        public string TEMP_SG_SIGLA { get; set; }
        public string TEMP_TX_CONTEUDO { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O ARQUIVO deve conter no minimo 1 caracteres e no máximo 100.")]
        public string TEMP_AQ_ARQUIVO { get; set; }
        public int TEMP_IN_ATIVO { get; set; }
        public string TEMP_TX_CONTEUDO_LIMPO { get; set; }
        public string TEMP_TX_CABECALHO { get; set; }
        public string TEMP_TX_CORPO { get; set; }
        public string TEMP_TX_DADOS { get; set; }
        public Nullable<System.DateTime> TEMP_DT_CRIACAO { get; set; }
        public Nullable<int> TEMP_IN_EDITAVEL { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}