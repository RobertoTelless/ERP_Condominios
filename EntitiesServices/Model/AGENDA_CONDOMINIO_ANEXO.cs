//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AGENDA_CONDOMINIO_ANEXO
    {
        public int ACAN_CD_ID { get; set; }
        public int AGCO_CD_ID { get; set; }
        public string ACAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> ACAN_DT_ANEXO { get; set; }
        public Nullable<int> ACAN_IN_TIPO { get; set; }
        public string ACAN_AQ_ARQUIVO { get; set; }
        public Nullable<int> ACAN_IN_ATIVO { get; set; }
    
        public virtual AGENDA_CONDOMINIO AGENDA_CONDOMINIO { get; set; }
    }
}