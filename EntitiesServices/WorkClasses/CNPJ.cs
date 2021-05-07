using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class CNPJ
    {
        public string RAZAO { get; set; }
        public string NOME { get; set; }
        public string CEP { get; set; }
        public string ENDERECO { get; set; }
        public string NUMERO { get; set; }
        public string BAIRRO { get; set; }
        public string CIDADE { get; set; }
        public Int32? UF { get; set; }
        public string INSCRICAO_ESTADUAL { get; set; }
        public string TELEFONE { get; set; }
        public string EMAIL { get; set; }
    }
}
