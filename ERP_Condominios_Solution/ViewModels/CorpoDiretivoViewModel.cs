using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CorpoDiretivoViewModel
    {
        [Key]
        public int CODI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FUNÇÃO obrigatorio")]
        public int FUCO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE INÍCIO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime CODI_DT_INICIO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CODI_DT_FINAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CODI_DT_SAIDA_REAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime CODI_DT_CADASTRO { get; set; }
        [StringLength(500, ErrorMessage = "OBSERVAÇÕES devem conter no máximo 500 caracteres.")]
        public string CODI_TX_OBSERVACOES { get; set; }
        public int CODI_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual FUNCAO_CORPO_DIRETIVO FUNCAO_CORPO_DIRETIVO { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}