using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class MovimentacaoAvulsaViewModel
    {
        public Int32 ProdutoInsumo { get; set; }

        [Required(ErrorMessage = "Campo OPERAÇÃO obrigatorio")]
        public Int32 MOVMT_IN_OPERACAO { get; set; }
        public Int32? MOVMT_IN_TIPO_MOVIMENTO_ENTRADA { get; set; }
        public Int32? MOVMT_IN_TIPO_MOVIMENTO_SAIDA { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        public DateTime MOVMT_DT_MOVIMENTO { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public Int32 FILI_CD_ID { get; set; }
        public Int32? FILI_DESTINO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo JUSTIFICATIVA obrigatorio")]
        public String MOVMT_DS_JUSTIFICATIVA { get; set; }


        public Int32? ProdutoInsumoEx { get; set; }
        public Int32? MOVMT_IN_OPERACAO_EX { get; set; }
        public Int32 FILI_CD_ID_EX { get; set; }
        public Int32? FORN_CD_ID { get; set; }
        public Int32? PROD_CD_ID { get; set; }
        public Int32? MAPR_CD_ID { get; set; }
        public Int32? QTDE_PROD { get; set; }
        public Int32? QTDE_MAPR { get; set; }


        public Int32[] REGISTROS { get; set; }
        public Int32[] QUANTIDADE { get; set; }

        public Int32? btnVolta { get; set; }
    }
}