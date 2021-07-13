using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class FuncionarioViewModel
    {
        [Key]
        public int FUNC_CD_ID { get; set; }
        public string FUNC_AQ_FOTO { get; set; }
        [Required(ErrorMessage = "Campo SITUAÇÂO obrigatorio")]
        public int SITU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SEXO obrigatorio")]
        public int SEXO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FUNÇÃO obrigatorio")]
        public Nullable<int> FNCA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ESCOLARIDADE obrigatorio")]
        public Nullable<int> ESCO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 100.")]
        public string FUNC_NM_NOME { get; set; }
        //[Required(ErrorMessage = "Campo DATA DE NASCIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime FUNC_DT_NASCIMENTO { get; set; }
        [Required(ErrorMessage = "Campo CPF obrigatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O CPF deve conter no minimo 1 caracteres e no máximo 20.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string FUNC_NR_CPF { get; set; }
        [Required(ErrorMessage = "Campo RG obrigatorio")]
        [StringLength(20, ErrorMessage = "O RG deve conter no máximo 20 caracteres.")]
        public string FUNC_NR_RG { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50 caracteres.")]
        public string FUNC_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve conter no máximo 50 caracteres.")]
        public string FINC_NR_CELULAR { get; set; }
        [StringLength(100, ErrorMessage = "O E-MAIL deve conter no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string FUNC_NM_EMAIL { get; set; }
        public Nullable<decimal> FUNC_NR_SALARIO { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO deve conter no máximo 100 caracteres.")]
        public string FUNC_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_CIDADE { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string FUNC_NR_CEP { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string FUNC_TX_OBSERVACOES { get; set; }
        public Nullable<int> FUNC_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_ADMISSAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_DEMISSAO { get; set; }
        [StringLength(500, ErrorMessage = "O MOTIVO DA DEMISSÃO deve conter no máximo 500 caracteres.")]
        public string FUNC_NM_MOTIVO_DEMISSAO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_EMISSAO_RG { get; set; }
        [StringLength(50, ErrorMessage = "O ÓRGÃO EMISSOR deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_ORGAO_RG { get; set; }
        public Nullable<int> FUNC_UF_FG_ID { get; set; }
        [StringLength(50, ErrorMessage = "A PROFISSÃO deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_PROFISSAO { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO PAI deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_PAI { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DA MÃE deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_MAE { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO CONJUGE deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_CONJUGE { get; set; }
        [StringLength(50, ErrorMessage = "A NATURALIDADE deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_NATURALIDADE { get; set; }
        public Nullable<int> FUNC_UF_NATURALIDADE_ID { get; set; }
        public Nullable<int> PAIS_CD_ID { get; set; }
        [StringLength(15, ErrorMessage = "O NIS deve conter no máximo 15 caracteres.")]
        public string FUNC_NR_NIS { get; set; }
        [StringLength(10, ErrorMessage = "A CTPS deve conter no máximo 10 caracteres.")]
        public string FUNC_NR_CTPS { get; set; }
        [StringLength(10, ErrorMessage = "A SÉRIE DA CTPS deve conter no máximo 50 caracteres.")]
        public string FUNC_NR_CTPS_SERIE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_CTPS_EMISSAO { get; set; }
        public Nullable<int> FUNC_UF_CTPS_ID { get; set; }
        [StringLength(15, ErrorMessage = "O TÌTULO DE ELEITOR deve conter no máximo 15 caracteres.")]
        public string FUNC_NR_TITULO_ELEITOR { get; set; }
        [StringLength(10, ErrorMessage = "A ZONA ELEITORAL deve conter no máximo 10 caracteres.")]
        public string FUNC_NR_ZONA { get; set; }
        [StringLength(10, ErrorMessage = "A SEÇÃO ELEITORAL deve conter no máximo 10 caracteres.")]
        public string FUNC_NR_SECAO { get; set; }
        [StringLength(50, ErrorMessage = "O MUNICÍPIO deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_MUNICIPIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_EMISSAO_TITULO { get; set; }
        [StringLength(15, ErrorMessage = "O CERT. RESERVISTA deve conter no máximo 15 caracteres.")]
        public string FUNC_NR_RESERVISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> FUNC_DT_RESERVISTA_EMISSAO { get; set; }
        [StringLength(15, ErrorMessage = "A SITUAÇÂO DO RESERVISTA deve conter no máximo 15 caracteres.")]
        public string FUNC_NM_RESERVISTA_SITUACAO { get; set; }
        [StringLength(15, ErrorMessage = "A O.M. RESERVISTA deve conter no máximo 15 caracteres.")]
        public string FUNC_NM_RESERVISTA_OM { get; set; }
        [StringLength(50, ErrorMessage = "A NACIONALIDADE deve conter no máximo 50 caracteres.")]
        public string FUNC_NM_NACIONALIDADE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual ESCOLARIDADE ESCOLARIDADE { get; set; }
        public virtual FUNCAO FUNCAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO_ANEXO> FUNCIONARIO_ANEXO { get; set; }
        public virtual SEXO SEXO { get; set; }
        public virtual SITUACAO SITUACAO { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        public virtual PAIS PAIS { get; set; }
        public virtual UF UF1 { get; set; }
        public virtual UF UF2 { get; set; }
        public virtual UF UF3 { get; set; }
    }
}