using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Work_Classes;
using Correios.Net;


namespace ModelServices.Interfaces.ExternalServices
{
    public interface IECT_Services 
    {
        Address GetAdressCEP(string CEP);
        Endereco GetAdressCEPService(string CEP);
    }
}
