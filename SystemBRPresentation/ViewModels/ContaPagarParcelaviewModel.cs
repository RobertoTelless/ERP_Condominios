using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class ContaPagarParcelaViewModel
    {
        [Key]
        public int CPPA_CD_ID { get; set; }
        public int CAPA_CD_ID { get; set; }
        public Nullable<int> CPPA_IN_PARCELA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CPPA_VL_VALOR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CPPA_DT_VENCIMENTO { get; set; }
        public Nullable<int> CPPA_IN_ATIVO { get; set; }
        public string CPPA_DS_DESCRICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CPPA_DT_QUITACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CPPA_VL_VALOR_PAGO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CPPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CPPA_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CPPA_VL_TAXAS { get; set; }
        public Nullable<int> CPPA_IN_QUITADA { get; set; }
        public string CPPA_NR_PARCELA { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<int> CPPA_IN_CHEQUE { get; set; }
        public string CPPA_NR_CHEQUE { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }

        public bool Cheque
        {
            get
            {
                if (CPPA_IN_CHEQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CPPA_IN_CHEQUE = (value == true) ? 1 : 0;
            }
        }

        public virtual CONTA_PAGAR CONTA_PAGAR { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
    }
}