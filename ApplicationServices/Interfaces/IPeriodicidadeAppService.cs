using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPeriodicidadeAppService : IAppServiceBase<PERIODICIDADE>
    {
        Int32 ValidateCreate(PERIODICIDADE item, USUARIO usuario);
        Int32 ValidateEdit(PERIODICIDADE item, PERIODICIDADE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(PERIODICIDADE item, USUARIO usuario);
        Int32 ValidateReativar(PERIODICIDADE item, USUARIO usuario);
        List<PERIODICIDADE> GetAllItens();
        PERIODICIDADE GetItemById(Int32 id);
        List<PERIODICIDADE> GetAllItensAdm();
    }
}
