using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ItemPedidoVendaViewModel
    {
        [Key]
        public int ITPE_CD_ID { get; set; }
        public int PEVE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int ITPE_QN_QUANTIDADE { get; set; }
        public int ITPE_IN_ATIVO { get; set; }
        public string ITPE_TX_OBSERVACOES { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> TIIM_CD_ID { get; set; }
        public Nullable<int> COIM_CD_ID { get; set; }

        public virtual PEDIDO_VENDA PEDIDO_VENDA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual COR_IMPRESSAO COR_IMPRESSAO { get; set; }
        public virtual TIPO_IMPRESSAO TIPO_IMPRESSAO { get; set; }
    }
}