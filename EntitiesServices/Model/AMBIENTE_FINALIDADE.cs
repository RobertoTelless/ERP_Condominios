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
    
    public partial class AMBIENTE_FINALIDADE
    {
        public int AMFI_CD_ID { get; set; }
        public int AMBI_CD_ID { get; set; }
        public int FIRE_CD_ID { get; set; }
        public int AMFI_IN_ATIVO { get; set; }
    
        public virtual AMBIENTE AMBIENTE { get; set; }
        public virtual FINALIDADE_RESERVA FINALIDADE_RESERVA { get; set; }
    }
}
