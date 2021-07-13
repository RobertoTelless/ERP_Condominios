using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class PedidoVendaViewModel
    {
        [Key]
        public int PEVE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public int CLIE_CD_ID { get; set; }

        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PEVE_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PEVE_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int PEVE_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_MUDANCA_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_APROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_CANCELAMENTO { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250.")]
        public string PEVE_DS_DESCRICAO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500.")]
        public string PEVE_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_VALIDADE { get; set; }
        public string PEVE_NM_FORMA_PAGAMENTO { get; set; }
        public string PEVE_TX_OBSERVACOES { get; set; }
        public int PEVE_IN_ATIVO { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PEVE_NR_NUMERO { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_PREVISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_ALTERACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PEVE_VL_VALOR { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMISSAO_VENDA> COMISSAO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_VENDA> ITEM_PEDIDO_VENDA { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_ANEXO> PEDIDO_VENDA_ANEXO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}