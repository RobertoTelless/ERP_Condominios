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
    
    public partial class RESERVA_COMENTARIO
    {
        public int RECO_CD_ID { get; set; }
        public int RESE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> RECO_DT_COMENTARIO { get; set; }
        public string RECO_DS_COMENTARIO { get; set; }
        public Nullable<int> RECO_IN_ATIVO { get; set; }
    
        public virtual RESERVA RESERVA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
