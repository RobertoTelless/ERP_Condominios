using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VagaViewModel
    {
        [Key]
        public int VAGA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE VAGA obrigatorio")]
        public int TIVA_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ANDAR obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O ANDAR deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string VAGA_NR_ANDAR { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string VAGA_NR_NUMERO { get; set; }
        public Nullable<int> VAGA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE ATRIBUIÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> VAGA_DT_ATRIBUICAO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string VAGA_DS_JUSTIFICATIVA { get; set; }
        public string VAGA_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual TIPO_VAGA TIPO_VAGA { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO> VEICULO { get; set; }

    }
}