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
    
    public partial class MATERIAL_ANEXO
    {
        public int MAAN_CD_ID { get; set; }
        public int MATE_CD_ID { get; set; }
        public string MAAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> MAAN_DT_ANEXO { get; set; }
        public Nullable<int> MAAN_IN_TIPO { get; set; }
        public string MAAN_AQ_ARQUIVO { get; set; }
        public Nullable<int> MAAN_IN_ATIVO { get; set; }
    
        public virtual MATERIAL MATERIAL { get; set; }
    }
}