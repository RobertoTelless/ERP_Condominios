using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IGrupoService : IServiceBase<GRUPO>
    {
        Int32 Create(GRUPO item, LOG log);
        Int32 Create(GRUPO item);
        Int32 Edit(GRUPO item, LOG log);
        Int32 Edit(GRUPO item);
        Int32 Delete(GRUPO item, LOG log);
        GRUPO GetItemById(Int32 id);
        GRUPO CheckExist(GRUPO item);
        List<GRUPO> GetAllItens();
        List<GRUPO> GetAllItensAdm();
    }
}
