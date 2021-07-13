using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBR_Presentation.ViewModels
{
    public class NotificacaoAnexoViewModel
    {
        public int NOAN_CD_ID { get; set; }
        public int NOTI_CD_ID { get; set; }
        public string NOAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> NOAN_DT_ANEXO { get; set; }
        public Nullable<int> NOAN_IN_TIPO { get; set; }
        public int NOAN_IN_ATIVO { get; set; }
        public string NOAN_AQ_ARQUIVO { get; set; }

        public virtual NOTIFICACAO NOTIFICACAO { get; set; }

    }
}