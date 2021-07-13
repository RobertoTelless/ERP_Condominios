using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class MovimentacaoAvulsaGridViewModel
    {
        public Int32? ProdutoInsumo { get; set; }
        public Int32? MOVMT_IN_OPERACAO { get; set; }
        public Int32? MOVMT_IN_TIPO_MOVIMENTO { get; set; }
        public Int32? MOVMT_IN_CHAVE_ORIGEM { get; set; }
        public DateTime? MOVMT_DT_MOVIMENTO_INICIAL { get; set; }
        public DateTime? MOVMT_DT_MOVIMENTO_FINAL { get; set; }
        public Int32? FILI_CD_ID { get; set; }
        public Int32? PROD_CD_ID { get; set; }
        public Int32? MAPR_CD_ID { get; set; }
    }
}