using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPlanoContaService : IServiceBase<PLANO_CONTA>
    {
        Int32 Create(PLANO_CONTA perfil, LOG log);
        Int32 Create(PLANO_CONTA perfil);
        Int32 Edit(PLANO_CONTA perfil, LOG log);
        Int32 Edit(PLANO_CONTA perfil);
        Int32 Delete(PLANO_CONTA perfil, LOG log);
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItens();
        List<PLANO_CONTA> GetAllItensAdm();
        PLANO_CONTA CheckExist(PLANO_CONTA item);
    }
}
