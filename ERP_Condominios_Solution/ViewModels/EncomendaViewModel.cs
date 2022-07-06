using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EncomendaViewModel
    {
        [Key]
        public int ENCO_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public Nullable<int> UNID_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> TIEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CHEGADA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE CHEGADA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENCO_DT_CHEGADA { get; set; }
        [StringLength(50, ErrorMessage = "O REMETENTE  deve conter no máximo 50 caracteres.")]
        public string ENCO_NM_REMETENTE { get; set; }
        [StringLength(50, ErrorMessage = "O ENTREGADOR deve conter no máximo 50 caracteres.")]
        public string ENCO_NM_ENTREGADOR { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A DESCRIÇÂO deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string ENCO_DS_ENCOMENDA { get; set; }
        [StringLength(6, ErrorMessage = "O CÓDIGO deve conter no máximo 6 caracteres.")]
        public string ENCO_CD_CODIGO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE ENTREGA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENCO_DT_ENTREGA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE BAIXA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENCO_DT_BAIXA { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DA PESSOA deve conter no máximo 50 caracteres.")]
        public string ENCO_NM_PESSOA { get; set; }
        public int ENCO_IN_CONFIRMADO { get; set; }
        public int ENCO_IN_ATIVO { get; set; }
        [StringLength(500, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 500 caracteres.")]
        public string ENCO_TX_OBSERVACOES { get; set; }
        public string ENCO_AQ_FOTO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string ENCO_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> ENCO_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE DEVOLUÇÃO Deve ser uma data válida")]
        public Nullable<System.DateTime> ENCO_DT_DEVOLUCAO { get; set; }
        public Nullable<int> ENCO_IN_STATUS_TROCA { get; set; }
        public string ENCO_CD_CODIGO_DIGITA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE RECUSA Deve ser uma data válida")]
        public Nullable<System.DateTime> ENCO_DT_RECUSA { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string ENCO_DS_JUSTIFICATIVA_RECUSA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA_ANEXO> ENCOMENDA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENCOMENDA_COMENTARIO> ENCOMENDA_COMENTARIO { get; set; }
        public virtual FORMA_ENTREGA FORMA_ENTREGA { get; set; }
        public virtual TIPO_ENCOMENDA TIPO_ENCOMENDA { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}