using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AutorizacaoViewModel
    {
        [Key]
        public int AUAC_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        public Nullable<int> GRPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> TIDO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O MOTIVO deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string AUAC_NM_VISITANTE { get; set; }
        [StringLength(50, ErrorMessage = "O DOCUMEMTO deve conter no máximo 50 caracteres.")]
        public string AUAC_NR_DOCUMENTO { get; set; }
        [Required(ErrorMessage = "Campo MOTIVO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string AUAC_DS_MOTIVO { get; set; }
        [StringLength(50, ErrorMessage = "A EMPRESA deve conter no máximo 50 caracteres.")]
        public string AUAC_NM_EMPRESA { get; set; }
        public int AUAC_IN_AVISO { get; set; }
        public int AUAC_IN_PERMANENTE { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> AUAC_IN_TIPO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VISITA deve ser uma data válida")]
        public Nullable<System.DateTime> AUAC_DT_VISITA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA LIMITE deve ser uma data válida")]
        public Nullable<System.DateTime> AUAC_DT_LIMITE { get; set; }
        [StringLength(250, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 250 caracteres.")]
        public string AUAC_DS_OBSERVACOES { get; set; }
        public int AUAC_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE INÍCIO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE INÍCIO deve ser uma data válida")]
        public Nullable<System.DateTime> AUAC_DT_INICIO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual GRAU_PARENTESCO GRAU_PARENTESCO { get; set; }
        public virtual TIPO_DOCUMENTO TIPO_DOCUMENTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENTRADA_SAIDA> ENTRADA_SAIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AUTORIZACAO_ACESSO_ANEXO> AUTORIZACAO_ACESSO_ANEXO { get; set; }
    }
}