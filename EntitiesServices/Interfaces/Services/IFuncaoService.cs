using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFuncaoService : IServiceBase<FUNCAO>
    {
        Int32 Create(FUNCAO item, LOG log);
        Int32 Create(FUNCAO item);
        Int32 Edit(FUNCAO item, LOG log);
        Int32 Edit(FUNCAO item);
        Int32 Delete(FUNCAO item, LOG log);
        FUNCAO GetItemById(Int32 id);
        List<FUNCAO> GetAllItens();
        List<FUNCAO> GetAllItensAdm();
    }
}
