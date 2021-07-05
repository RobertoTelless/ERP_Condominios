using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TelefoneViewModel
    {
        [Key]
        public int TELE_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CATE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string TELE_NM_NOME { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50 caracteres.")]
        public string TELE_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve conter no máximo 50 caracteres.")]
        public string TELE_NR_CELULAR { get; set; }
        [StringLength(50, ErrorMessage = "O WHATSAPP deve conter no máximo 50 caracteres.")]
        public string TELE_NR_WHATSAPP { get; set; }
        [StringLength(150, ErrorMessage = "O E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string TELE_NM_EMAIL { get; set; }
        public Nullable<int> TELE_IN_ATIVO { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50 caracteres.")]
        public string TELE_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string TELE_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string TELE_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string TELE_NR_CEP { get; set; }
        [StringLength(5000, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 5000 caracteres.")]
        public string TELE_TX_OBSERVACOES { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_TELEFONE CATEGORIA_TELEFONE { get; set; }
        public virtual UF UF { get; set; }
    }
}