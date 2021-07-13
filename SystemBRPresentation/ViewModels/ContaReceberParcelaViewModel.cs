using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class ContaReceberParcelaViewModel
    {
        [Key]
        public int CRPA_CD_ID { get; set; }
        public int CARE_CD_ID { get; set; }
        public Nullable<int> CRPA_IN_PARCELA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_VALOR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRPA_DT_VENCIMENTO { get; set; }
        public string CRPA_DS_DESCRICAO { get; set; }
        public Nullable<int> CRPA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRPA_DT_QUITACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_RECEBIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPA_VL_TAXAS { get; set; }
        public Nullable<int> CRPA_IN_QUITADA { get; set; }
        public string CRPA_NR_PARCELA { get; set; }

        public virtual CONTA_RECEBER CONTA_RECEBER { get; set; }

    }
}