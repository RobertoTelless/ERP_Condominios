using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class ContaPagarViewModel
    {
        [Key]
        public int CAPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo RESPONSÁVEL obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PLCO_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<int> TIFA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORNECEDOR obrigatorio")]
        public Nullable<int> FORN_CD_ID { get; set; }
        public Nullable<int> TITA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE PAGAMENTO obrigatorio")]
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CENTRO DE CUSTO obrigatorio")]
        public Nullable<int> CECU_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Required(ErrorMessage = "Campo DATA DE LANÇAMENTO obrigatorio")]
        public System.DateTime CAPA_DT_LANCAMENTO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_VALOR { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 caracteres e no máximo 250.")]
        public string CAPA_DS_DESCRICAO { get; set; }
        public int CAPA_IN_TIPO_LANCAMENTO { get; set; }
        public string CAPA_NM_FAVORECIDO { get; set; }
        [StringLength(50, ErrorMessage = "A FORMA DE PAGAMENTO deve conter no máximo 50.")]
        public string CAPA_NM_FORMA_PAGAMENTO { get; set; }
        public int CAPA_IN_LIQUIDADA { get; set; }
        public int CAPA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CAPA_DT_VENCIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_VALOR_PAGO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CAPA_DT_LIQUIDACAO { get; set; }
        public Nullable<int> CAPA_NR_ATRASO { get; set; }
        public string CAPA_TX_OBSERVACOES { get; set; }
        public Nullable<int> CAPA_IN_PARCELADA { get; set; }
        public Nullable<int> CAPA_IN_PARCELAS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CAPA_DT_INICIO_PARCELAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_PARCELADO { get; set; }
        [StringLength(50, ErrorMessage = "O NÙMERO DO DOCUMENTO deve conter no máximo 50.")]
        public string CAPA_NR_DOCUMENTO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE COMPETÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CAPA_DT_COMPETENCIA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_TAXAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_SALDO { get; set; }
        public Nullable<int> CAPA_IN_PAGA_PARCIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CAPA_VL_PARCIAL { get; set; }
        public Nullable<int> PECO_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID_1 { get; set; }
        public Nullable<int> CAPA_IN_CHEQUE { get; set; }
        public string CAPA_NR_CHEQUE { get; set; }
        public int CAPA_IN_RECORRENTE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CAPA_DT_INICIO_RECORRENCIA { get; set; }
        public Nullable<int> CECU_CD_RATEIO { get; set; }
        public Nullable<int> CAPA_VL_PERCENTUAL { get; set; }
        public Nullable<int> CAPA_IN_TIPO_DOC { get; set; }
        public Nullable<int> CAPA_IN_PAGAMENTO { get; set; }

        public bool Cheque
        {
            get
            {
                if (CAPA_IN_CHEQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPA_IN_CHEQUE = (value == true) ? 1 : 0;
            }
        }

        public bool Liquidada
        {
            get
            {
                if (CAPA_IN_LIQUIDADA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPA_IN_LIQUIDADA = (value == true) ? 1 : 0;
            }
        }
        public bool Parcial
        {
            get
            {
                if (CAPA_IN_PAGA_PARCIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPA_IN_PAGA_PARCIAL = (value == true) ? 1 : 0;
            }
        }
        public bool Parcelado
        {
            get
            {
                if (CAPA_IN_PARCELADA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CAPA_IN_PARCELADA = (value == true) ? 1 : 0;
            }
        }

        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_ANEXO> CONTA_PAGAR_ANEXO { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_PARCELA> CONTA_PAGAR_PARCELA { get; set; }
        public virtual PEDIDO_COMPRA PEDIDO_COMPRA { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
        public virtual PLANO_CONTA PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_RATEIO> CONTA_PAGAR_RATEIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_TAG> CONTA_PAGAR_TAG { get; set; }
        public virtual TIPO_FAVORECIDO TIPO_FAVORECIDO { get; set; }
        public virtual TIPO_TAG TIPO_TAG { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO1 { get; set; }
    }
}