using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class NoticiaViewModel
    {
        [Key]
        public int NOTC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE EMISSÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE EMISSÂO Deve ser uma data válida")]
        public System.DateTime NOTC_DT_EMISSAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE VALIDADE Deve ser uma data válida")]
        public System.DateTime NOTC_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O TÍTULO deve ter no minimo 1 caractere e no máximo 100.")]
        public string NOTC_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo AUTOR obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O AUTOR deve ter no minimo 1 caractere e no máximo 50.")]
        public string NOTC_NM_AUTOR { get; set; }
        [Required(ErrorMessage = "Campo DATA DE AUTORIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE AUTORIA Deve ser uma data válida")]
        public Nullable<System.DateTime> NOTC_DT_DATA_AUTOR { get; set; }
        public string NOTC_TX_TEXTO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve ter máximo 250 caracteres.")]
        public string NOTC_AQ_ARQUIVO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO LINK deve ter máximo 250 caracteres.")]
        public string NOTC_LK_LINK { get; set; }
        public int NOTC_NR_ACESSO { get; set; }
        public int NOTC_IN_ATIVO { get; set; }
        public string NOTC_AQ_FOTO { get; set; }
        public string NOTC_NM_ORIGEM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}