using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ItemPedidoCompraViewModel
    {
        [Key]
        public int ITPC_CD_ID { get; set; }
        public int PECO_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        public Nullable<int> MAPR_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }

        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int ITPC_QN_QUANTIDADE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ITPC_VL_PRECO_SELECIONADO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> ITPC_NR_QUANTIDADE_REVISADA { get; set; }
        public string ITPC_TX_OBSERVACOES { get; set; }
        public int ITPC_IN_ATIVO { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ITPC_DT_COTACAO { get; set; }
        public Nullable<int> ITPC_IN_TIPO { get; set; }
        public Nullable<int> PROD_VL_CUSTO { get; set; }
        public Nullable<int> PROD_VL_MARKUP_PADRAO { get; set; }
        public string ITPC_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> ITPC_NR_QUANTIDADE_RECEBIDA { get; set; }

        public virtual FORNECEDOR FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual MATERIA_PRIMA MATERIA_PRIMA { get; set; }
        public virtual PEDIDO_COMPRA PEDIDO_COMPRA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
    }
}