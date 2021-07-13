using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ServicoViewModel
    {
        [Key]
        public int SERV_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CASE_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> NBSE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string SERV_NM_NOME { get; set; }
        public System.DateTime SERV_DT_CADASTRO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 5000.")]
        public string SERV_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> SERV_VL_PRECO { get; set; }
        [StringLength(10, ErrorMessage = "O CÓDIGO deve conter no máximo 10.")]
        public string SERV_CD_CODIGO { get; set; }
        public int SERV_IN_ATIVO { get; set; }
        public string SERV_TX_OBSERVACOES { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_SERVICO CATEGORIA_SERVICO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        public virtual NOMENCLATURA_BRAS_SERVICOS NOMENCLATURA_BRAS_SERVICOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO_ANEXO> SERVICO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO_TABELA_PRECO> SERVICO_TABELA_PRECO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
    }
}