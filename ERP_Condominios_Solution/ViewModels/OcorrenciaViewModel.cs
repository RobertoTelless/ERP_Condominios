using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class OcorrenciaViewModel
    {
        [Key]
        public int OCOR_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CAOC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DA OCORRÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DA OCORRÊNCIA Deve ser uma data válida")]
        public System.DateTime OCOR_DT_OCORRENCIA { get; set; }
        public int OCOR_IN_STATUS { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O TÍTULO deve ter no minimo 1 e no máximo 50 caracteres.")]
        public string OCOR_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve ter no minimo 1 e no máximo 5000 caracteres.")]
        public string OCOR_TX_TEXTO { get; set; }
        [StringLength(1000, ErrorMessage = "A JUSTIFICATIVA deve ter no máximo 1000 caracteres.")]
        public string OCOR_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE ENCERRAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> OCOR_DT_ENCERRAMENTO { get; set; }
        [StringLength(1000, ErrorMessage = "O TEXTO DA ENCERRAMENTO deve ter no máximo 1000 caracteres.")]
        public string OCOR_TX_ENCERRAMENTO { get; set; }
        public System.DateTime OCOR_DT_CADASTRO { get; set; }
        public int OCOR_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_OCORRENCIA CATEGORIA_OCORRENCIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA_ANEXO> OCORRENCIA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OCORRENCIA_COMENTARIO> OCORRENCIA_COMENTARIO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}