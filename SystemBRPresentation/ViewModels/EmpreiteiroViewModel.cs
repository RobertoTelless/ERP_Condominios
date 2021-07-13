using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_ROEngenharia_Presentation.ViewModels
{
    public class EmpreiteiroViewModel
    {
        [Key]
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo RAMO DE ATIVIDADE obrigatorio")]
        public Nullable<int> RAAT_CD_ID { get; set; }
        public string EMPR_AQ_FOTO { get; set; }
        public Nullable<int> EMPR_IN_STATUS { get; set; }
        public Nullable<int> EMPR_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string EMPR_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100.")]
        public string EMPR_NM_RAZAO_SOCIAL { get; set; }
        [StringLength(20, MinimumLength = 11, ErrorMessage = "O CPF deve conter no minimo 11 caracteres e no máximo 20.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string EMPR_NR_CPF { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string EMPR_NR_CNPJ { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O RG deve conter no minimo 1 caracteres e no máximo 20.")]
        public string EMPR_NR_RG { get; set; }
        [StringLength(50, ErrorMessage = "O YTELEFONE  deve conter no máximo 50.")]
        public string EMPR_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR  deve conter no máximo 50.")]
        public string EMPR_NR_CELULAR { get; set; }
        [StringLength(50, ErrorMessage = "O FAX  deve conter no máximo 50.")]
        public string EMPR_NR_FAX { get; set; }
        [StringLength(50, ErrorMessage = "O CONTATO  deve conter no máximo 50.")]
        public string EMPR_NM_CONTATO { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string EMPR_NM_EMAIL { get; set; }
        [StringLength(100, ErrorMessage = "O WEBSITE  deve conter no máximo 100.")]
        public string EMPR_NM_WEBSITE { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO  deve conter no máximo 100.")]
        public string EMPR_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO  deve conter no máximo 50.")]
        public string EMPR_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE  deve conter no máximo 50.")]
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP  deve conter no máximo 10.")]
        public string EMPR_NR_CEP { get; set; }
        public string EMPR_TX_OBSERVACAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPREITEIRO_ANEXO> EMPREITEIRO_ANEXO { get; set; }
        public virtual RAMO_ATIVIDADE RAMO_ATIVIDADE { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
    }
}