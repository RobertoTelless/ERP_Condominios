using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class PedidoCompraAcompanhamentoViewModel
    {
        [Key]
        public int PCAT_CD_ID { get; set; }
        public int PECO_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PCAT_DT_ACOMPANHAMENTO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O ACOMPANHAMENTO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string PCAT_DS_ACOMPANHAMENTO { get; set; }
        public int PCAT_IN_ATIVO { get; set; }

        //DT ENCERRAMENTO

        public virtual PEDIDO_COMPRA PEDIDO_COMPRA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}