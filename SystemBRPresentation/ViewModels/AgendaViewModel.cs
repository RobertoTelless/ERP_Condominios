using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class AgendaViewModel
    {
        [Key]
        public int AGEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAAG_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime AGEN_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo HORA obrigatorio")]
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public System.TimeSpan AGEN_HR_HORA { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(50, ErrorMessage = "O TÍTULO deve conter no máximo 50.")]
        public string AGEN_NM_TITULO { get; set; }
        [StringLength(5000, ErrorMessage = "A DESCRIÇÂO deve conter no máximo 5000.")]
        public string AGEN_DS_DESCRICAO { get; set; }
        public int AGEN_IN_ATIVO { get; set; }
        public Nullable<int> AGEN_CD_USUARIO { get; set; }
        public string AGEN_TX_OBSERVACOES { get; set; }
        public Nullable<int> AGEN_IN_STATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA_ANEXO> AGENDA_ANEXO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_AGENDA CATEGORIA_AGENDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}