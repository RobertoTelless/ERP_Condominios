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
    
    public partial class MATERIAL_MOVIMENTO
    {
        public int MAMO_CD_ID { get; set; }
        public int MATE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public System.DateTime MAMO_DT_MOVIMENTO { get; set; }
        public int MAMO_IN_TIPO_MOVIMENTO { get; set; }
        public int MAMO_QN_QUANTIDADE { get; set; }
        public int MAMO_IN_ATIVO { get; set; }
        public string MAMO_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> MAMO_QN_ANTES { get; set; }
        public Nullable<int> MAMO_QN_DEPOIS { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual MATERIAL MATERIAL { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
