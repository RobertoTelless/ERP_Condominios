using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MudancaViewModel
    {
        [Key]
        public int SOMU_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> SOMU_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DA MUDANÇA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DA MUDANÇA Deve ser uma data válida")]
        public System.DateTime SOMU_DT_MUDANCA { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> SOMU_NR_DURACAO { get; set; }
        [Required(ErrorMessage = "Campo ENTRADA/SAÌDA obrigatorio")]
        public int SOMU_IN_ENTRADA_SAIDA { get; set; }
        [StringLength(500, ErrorMessage = "AS OBSERVAÇÕES devem conter no máximo 500 caracteres.")]
        public string SOMU_DS_OBSERVACOES { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime SOMU_DT_CADASTRO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE EXECUÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> SOMU_DT_EXECUCAO_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE EXECUÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> SOMU_DT_EXECUCAO_FINAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE APROVAÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> SOMU_DT_APROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE VETO Deve ser uma data válida")]
        public Nullable<System.DateTime> SOMU_DT_VETADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CANCELAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> SOMU_DT_SUSPENSA { get; set; }
        [StringLength(50, ErrorMessage = "A PLACA  deve conter no máximo 50 caracteres.")]
        public string SOMU_NR_PLACA_CAMINHAO { get; set; }
        [StringLength(50, ErrorMessage = "A EMPRESA deve conter no máximo 50 caracteres.")]
        public string SOMU_NM_EMPRESA_MUDANCA { get; set; }
        public int SOMU_IN_APROVACAO { get; set; }
        public int SOMU_IN_EXECUTADA { get; set; }
        public int SOMU_IN_VETADA { get; set; }
        public Nullable<int> SOMU_IN_ANALISE { get; set; }
        public int SOMU_IN_SUSENSA { get; set; }
        public Nullable<int> SOMU_IN_STATUS { get; set; }
        public int SOMU_IN_ATIVO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string SOMU_DS_JUSTIFICATIVA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_CONDOMINIO> AGENDA_CONDOMINIO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA_ANEXO> SOLICITACAO_MUDANCA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA_COMENTARIO> SOLICITACAO_MUDANCA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITACAO_MUDANCA_MOVIMENTO> SOLICITACAO_MUDANCA_MOVIMENTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}