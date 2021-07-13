using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class UsuarioAnexoViewModel
    {
        public int USAN_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public string USAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> USAN_DT_ANEXO { get; set; }
        public Nullable<int> USAN_IN_TIPO { get; set; }
        public int USAN_IN_ATIVO { get; set; }
        public string USAN_AQ_ARQUIVO { get; set; }

        public virtual USUARIO USUARIO { get; set; }

    }
}