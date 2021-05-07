using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoContaService : IServiceBase<TIPO_CONTA>
    {
        Int32 Create(TIPO_CONTA item, LOG log);
        Int32 Create(TIPO_CONTA item);
        Int32 Edit(TIPO_CONTA item, LOG log);
        Int32 Edit(TIPO_CONTA item);
        Int32 Delete(TIPO_CONTA item, LOG log);
        TIPO_CONTA GetItemById(Int32 id);
        List<TIPO_CONTA> GetAllItens();
        List<TIPO_CONTA> GetAllItensAdm();
    }
}
