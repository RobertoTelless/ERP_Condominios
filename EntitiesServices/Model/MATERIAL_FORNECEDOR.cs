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
    
    public partial class MATERIAL_FORNECEDOR
    {
        public int MAFO_CD_ID { get; set; }
        public int MATE_CD_ID { get; set; }
        public int FORN_CD_ID { get; set; }
        public Nullable<int> MAFO_IN_ATIVO { get; set; }
    
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual MATERIAL MATERIAL { get; set; }
    }
}
