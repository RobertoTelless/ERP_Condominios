using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EquipamentoViewModel
    {
        [Key]
        public int EQUI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAEQ_CD_ID { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 20.")]
        public string EQUI_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string EQUI_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "O DESCRIÇÂO deve conter no máximo 500.")]
        public string EQUI_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE COMPRA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> EQUI_DT_COMPRA { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EQUI_VL_VALOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> EQUI_NR_VIDA_UTIL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> EQUI_DT_BAIXA { get; set; }
        [StringLength(250, ErrorMessage = "O MOTIVO DA BAIXA deve conter no máximo 250.")]
        public string EQUI_DS_MOTIVO_BAIXA { get; set; }
        public int EQUI_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> EQUI_DT_CADASTRO { get; set; }
        public string EQUI_AQ_FOTO { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> EQUI_DT_MANUTENCAO { get; set; }
        public Nullable<int> EQUI_IN_AVISA_MANUTENCAO { get; set; }
        public string EQUI_TX_OBSERVACOES { get; set; }
        public Nullable<int> EQUI_IN_AVISA_DEPRECIACAO { get; set; }

        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> Valor
        {
            get
            {
                return EQUI_VL_VALOR;
            }
            set
            {
                EQUI_VL_VALOR = value;
            }
        }
        public bool AvisaManutencao
        {
            get
            {
                if (EQUI_IN_AVISA_MANUTENCAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                EQUI_IN_AVISA_MANUTENCAO = (value == true) ? 1 : 0;
            }
        }
        public bool AvisaDepreciacao
        {
            get
            {
                if (EQUI_IN_AVISA_DEPRECIACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                EQUI_IN_AVISA_DEPRECIACAO = (value == true) ? 1 : 0;
            }
        }

        public virtual CATEGORIA_EQUIPAMENTO CATEGORIA_EQUIPAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPAMENTO_ANEXO> EQUIPAMENTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPAMENTO_MANUTENCAO> EQUIPAMENTO_MANUTENCAO { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
    }
}