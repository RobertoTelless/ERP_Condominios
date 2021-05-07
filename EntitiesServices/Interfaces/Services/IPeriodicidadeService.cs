using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPeriodicidadeService : IServiceBase<PERIODICIDADE>
    {
        Int32 Create(PERIODICIDADE perfil, LOG log);
        Int32 Create(PERIODICIDADE perfil);
        Int32 Edit(PERIODICIDADE perfil, LOG log);
        Int32 Edit(PERIODICIDADE perfil);
        Int32 Delete(PERIODICIDADE perfil, LOG log);
        PERIODICIDADE GetItemById(Int32 id);
        List<PERIODICIDADE> GetAllItens();
        List<PERIODICIDADE> GetAllItensAdm();
    }
}
