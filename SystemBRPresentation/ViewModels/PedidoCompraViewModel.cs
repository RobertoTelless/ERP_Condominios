using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class PedidoCompraViewModel
    {
        [Key]
        public int PECO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }

        public Nullable<decimal> VALOR_TOTAL { get; set; }

        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PECO_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PECO_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int PECO_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PECO_DT_APROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PECO_DT_CANCELAMENTO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500.")]
        public string PECO_DS_JUSTIFICATIVA { get; set; }
        [StringLength(50, ErrorMessage = "A NOTA FISCAL deve conter no máximo 50.")]
        public string PECO_NR_NOTA_FISCAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PECO_DT_ALTERACAO { get; set; }
        public string PECO_TX_OBSERVACOES { get; set; }
        public int PECO_IN_ATIVO { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PECO_NR_NUMERO { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PECO_DT_PREVISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PECO_DT_FINAL { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        public string PECO_TX_OBSERVACAO { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250.")]
        public string PECO_DS_DESCRICAO { get; set; }
        public Int32? FORN_CD_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_COMPRA> ITEM_PEDIDO_COMPRA { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA_ANEXO> PEDIDO_COMPRA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA_ACOMPANHAMENTO> PEDIDO_COMPRA_ACOMPANHAMENTO { get; set; }
        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
    }
}