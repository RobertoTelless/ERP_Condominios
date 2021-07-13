using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TarefaViewModel
    {
        [Key]
        public int TARE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime TARE_DT_CADASTRO { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string TARE_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 1000.")]
        public string TARE_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> TARE_DT_ESTIMADA { get; set; }
        public int TARE_IN_STATUS { get; set; }
        [Required(ErrorMessage = "Campo PRIORIDADE obrigatorio")]
        public int TARE_IN_PRIORIDADE { get; set; }
        public int TARE_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> TARE_DT_REALIZADA { get; set; }
        public string TARE_TX_OBSERVACOES { get; set; }
        [StringLength(150, ErrorMessage = "O LOCAL deve conter no máximo 150.")]
        public string TARE_NM_LOCAL { get; set; }
        public Nullable<int> TARE_IN_AVISA { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> TITR_CD_ID { get; set; }
        public Nullable<int> PETA_CD_ID { get; set; }
        public Nullable<int> TARE_NR_PERIODICIDADE_QUANTIDADE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ANEXO> TAREFA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_NOTIFICACAO> TAREFA_NOTIFICACAO { get; set; }
        public virtual TIPO_TAREFA TIPO_TAREFA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_VINCULO> TAREFA_VINCULO { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
    }
}