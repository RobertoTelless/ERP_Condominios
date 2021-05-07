using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class PRODUTOS_MAIS_VENDIDOS
    {
        public Int32 PROD_CD_ID { get; set; }
        public String PROD_NM_NOME { get; set; }
        public String PROD_CD_CODIGO { get; set; }
        public String PROD_NR_BARCODE { get; set; }
        public Int32 PRMV_QN_QUANTIDADE { get; set; }
        public DateTime? PRMV_ULTIMA_VENDA { get; set; }
        public String PROD_DS_DESCRICAO { get; set; }

        //Necessários para filtro
        public Int32? CAPR_CD_ID { get; set; }
        public Int32? SCPR_CD_ID { get; set; }
    }
}
