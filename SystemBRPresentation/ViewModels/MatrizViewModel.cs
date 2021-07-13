using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class MatrizViewModel
    {
        [Key]
        public int MATR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string MATR_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100.")]
        public string MATR_NM_RAZAO { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string MATR_NR_CNPJ { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string MATR_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string MATR_NR_TELEFONES { get; set; }
        [StringLength(50, ErrorMessage = "O CONTATO deve conter no máximo 50.")]
        public string MATR_NM_CONTATOS { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50.")]
        public string MATR_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50.")]
        public string MATR_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50.")]
        public string MATR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10.")]
        public string MATR_NR_CEP { get; set; }
        public System.DateTime MATR_DT_CADASTRO { get; set; }
        public int MATR_IN_ATIVO { get; set; }
        public Nullable<int> TIPE_CD_ID { get; set; }
        public Nullable<int> CRTR_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 50.")]
        public string MATR_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO MUNICIAPAL deve conter no máximo 50.")]
        public string MATR_NR_INSCRICAO_MUNICIPAL { get; set; }
        public Nullable<int> MATR_IN_IE_ISENTO { get; set; }
        [StringLength(50, ErrorMessage = "O CNAE deve conter no máximo 50.")]
        public string MATR_NR_CNAE { get; set; }
        [StringLength(100, ErrorMessage = "O WEBSITE deve conter no máximo 100.")]
        public string MATR_NM_WEBSITE { get; set; }
        [StringLength(50, ErrorMessage = "O CELURAR deve conter no máximo 50.")]
        public string MATR_NR_CELULAR { get; set; }
        [StringLength(250, ErrorMessage = "O LOGOTIPO deve conter no máximo 250.")]
        public string MATR_AQ_LOGOTIPO { get; set; }
        [StringLength(50, ErrorMessage = "O RG deve conter no máximo 50.")]
        public string MATR_NR_RG { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CPF deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string MATR_NR_CPF { get; set; }
        public string MATR_NR_CEP_BUSCA { get; set; }
        public string MATR_SG_UF { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CODIGO_REGIME_TRIBUTARIO CODIGO_REGIME_TRIBUTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FILIAL> FILIAL { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
    }
}