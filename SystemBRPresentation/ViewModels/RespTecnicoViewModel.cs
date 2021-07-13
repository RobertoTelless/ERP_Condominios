using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_ROEngenharia_Presentation.ViewModels
{
    public class RespTecnicoViewModel
    {
        [Key]
        public int RETE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo RAMO DE ATIVIDADE obrigatorio")]
        public Nullable<int> RAAT_CD_ID { get; set; }
        public string RETE_AQ_FOTO { get; set; }
        public Nullable<int> RETE_IN_STATUS { get; set; }
        public Nullable<int> RETE_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string RETE_NM_NOME { get; set; }
        [StringLength(20, MinimumLength = 11, ErrorMessage = "O CPF deve conter no minimo 11 caracteres e no máximo 20.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string RETE_NR_CPF { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string RETE_NR_CNPJ { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O RG deve conter no minimo 1 caracteres e no máximo 20.")]
        public string RETE_NR_RG { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string RETE_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR  deve conter no máximo 50.")]
        public string RETE_NR_CELULAR { get; set; }
        [StringLength(50, ErrorMessage = "O FAX  deve conter no máximo 50.")]
        public string RETE_NR_FAX { get; set; }
        [StringLength(50, ErrorMessage = "O CONTATO  deve conter no máximo 50.")]
        public string RETE_NM_CONTATO { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string RETE_NM_EMAIL { get; set; }
        [StringLength(100, ErrorMessage = "O WEBSITE  deve conter no máximo 100.")]
        public string RETE_NM_WEBSITE { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO  deve conter no máximo 100.")]
        public string RETE_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO  deve conter no máximo 50.")]
        public string RETE_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE  deve conter no máximo 50.")]
        public string RETE_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP  deve conter no máximo 10.")]
        public string RETE_NR_CEP { get; set; }
        public string RETE_TX_OBSERVACAO { get; set; }

        public virtual RAMO_ATIVIDADE RAMO_ATIVIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESPONSAVEL_TECNICO_ANEXO> RESPONSAVEL_TECNICO_ANEXO { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
    }
}