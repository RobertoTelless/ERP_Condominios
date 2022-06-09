using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Work_Classes
{
    public class MontagemGrupo
    {
        public int GRUP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public string GRUP_NM_NOME { get; set; }
        public System.DateTime GRUP_DT_CADASTRO { get; set; }
        public int GRUP_IN_ATIVO { get; set; }

        public Int32? SEXO { get; set; }
        public string NOME { get; set; }
        public Int32? ID { get; set; }
        public string CIDADE { get; set; }
        public Nullable<System.DateTime> DATA_NASC { get; set; }
        public Int32? UF { get; set; }
        public Int32? CATEGORIA { get; set; }
        public Int32? STATUS { get; set; }
        public String LINK { get; set; }
        public Int32? GRUPO { get; set; }
        public String MODELO { get; set; }
        public string DIA { get; set; }
        public string MES { get; set; }
        public string ANO { get; set; }

    }
}
