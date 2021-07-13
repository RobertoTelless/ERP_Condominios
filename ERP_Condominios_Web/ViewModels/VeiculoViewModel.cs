using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VeiculoViewModel
    {
        [Key]
        public int VEIC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE UNIDADE obrigatorio")]
        public int TIVE_CD_ID { get; set; }
        public Nullable<int> VAGA_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PLACA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A PLACA deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string VEIC_NM_PLACA { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50 caracteres.")]
        public string VEIC_NM_MARCA { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250 caracteres.")]
        public string VEIC_DS_DESCRICAO { get; set; }
        [StringLength(50, ErrorMessage = "A COR deve conter no máximo 50 caracteres.")]
        public string VEIC_NM_COR { get; set; }
        [StringLength(10, ErrorMessage = "O ANO deve conter no máximo 10 caracteres.")]
        public string VEIC_NR_ANO { get; set; }
        [StringLength(500, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 500 caracteres.")]
        public string VEIC_DS_OBSERVACOES { get; set; }
        [StringLength(250, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 250 caracteres.")]
        public string VEIC_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime VEIC_DT_CADASTRO { get; set; }
        public int VEIC_IN_ATIVO { get; set; }
        public Nullable<int> VEIC_IN_CONFIRMA_VAGA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CONFIRMAÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> VEIC_DT_CONFIRMACAO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve conter no máximo 250 caracteres.")]
        public string VEIC_AQ_FOTO { get; set; }
        public string VEIC_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual TIPO_VEICULO TIPO_VEICULO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual VAGA VAGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VEICULO_ANEXO> VEICULO_ANEXO { get; set; }
    }
}