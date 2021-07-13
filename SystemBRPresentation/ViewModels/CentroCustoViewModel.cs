using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class CentroCustoViewModel
    {
        [Key]
        public int CECU_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 caracteres e no máximo 10.")]
        public string CECU_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CECU_NM_NOME { get; set; }
        public int CECU_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo GRUPO obrigatorio")]
        public Nullable<int> GRUP_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBGRUPO obrigatorio")]
        public Nullable<int> SUBG_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> CECU_IN_TIPO { get; set; }
        [Required(ErrorMessage = "Campo MOVIMENTO obrigatorio")]
        public Nullable<int> CECU_IN_MOVTO { get; set; }
        public string CECU_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual GRUPO GRUPO { get; set; }
        public virtual SUBGRUPO SUBGRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_RATEIO> CONTA_PAGAR_RATEIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRATO> CONTRATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
    }
}