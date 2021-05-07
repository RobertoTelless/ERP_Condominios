using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICargoAppService : IAppServiceBase<CARGO>
    {
        Int32 ValidateCreate(CARGO item, USUARIO usuario);
        Int32 ValidateEdit(CARGO item, CARGO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CARGO item, CARGO itemAntes);
        Int32 ValidateDelete(CARGO item, USUARIO usuario);
        Int32 ValidateReativar(CARGO item, USUARIO usuario);
        List<CARGO> GetAllItens();
        List<CARGO> GetAllItensAdm();
        CARGO GetItemById(Int32 id);
    }
}
