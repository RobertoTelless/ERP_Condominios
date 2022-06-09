using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemWidgetViewModel
    {
        public DateTime? DataMensagem { get; set; }
        public String NomeUsuario { get; set; }
        public Int32 IdMensagem { get; set; }
        public Int32? TipoMensagem { get; set; }
        public String Descrição { get; set; }
        public Int32? FlagUrgencia { get; set; }
        public String Categoria { get; set; }
        public String NomeCliente { get; set; }
        public String Status { get; set; }
    }
}