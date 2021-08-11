using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EntradaSaidaViewModel
    {
        [Key]
        public int ENSA_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> AUAC_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string ENSA_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DOCUMENTO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O DOCUMENTO deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string ENSA_NR_DOCUMENTO { get; set; }
        [StringLength(50, ErrorMessage = "A EMPRESA deve conter no máximo 50 caracteres.")]
        public string ENSA_NM_EMPRESA { get; set; }
        [StringLength(50, ErrorMessage = "O MOTIVO deve conter no máximo 50 caracteres.")]
        public string ENSA_NM_MOTIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA D ENTRADA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE ENTRADA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENSA_DT_ENTRADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE SAÍDA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENSA_DT_SAIDA { get; set; }
        public string ENSA_AQ_FOTO { get; set; }
        [StringLength(500, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 500 caracteres.")]
        public string ENSA_DS_OBSERVACOES { get; set; }
        public int ENSA_IN_CONFIRMA { get; set; }
        public int ENSA_IN_LISTA_NEGRA { get; set; }
        public int ENSA_IN_STATUS { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string ENSA_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> GRPA_CD_ID { get; set; }
        public Nullable<int> ENSA_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual AUTORIZACAO_ACESSO AUTORIZACAO_ACESSO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENTRADA_SAIDA_COMENTARIO> ENTRADA_SAIDA_COMENTARIO { get; set; }
        public virtual GRAU_PARENTESCO GRAU_PARENTESCO { get; set; }
    }
}