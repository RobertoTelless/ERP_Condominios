using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPlanoContaAppService : IAppServiceBase<PLANO_CONTA>
    {
        Int32 ValidateCreate(PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateEdit(PLANO_CONTA item, PLANO_CONTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateReativar(PLANO_CONTA item, USUARIO usuario);
        List<PLANO_CONTA> GetAllItens();
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItensAdm();
        PLANO_CONTA CheckExist(PLANO_CONTA obj);
    }
}
