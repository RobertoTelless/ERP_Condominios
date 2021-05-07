using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFuncaoAppService : IAppServiceBase<FUNCAO>
    {
        Int32 ValidateCreate(FUNCAO item, USUARIO usuario);
        Int32 ValidateEdit(FUNCAO item, FUNCAO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(FUNCAO item, USUARIO usuario);
        Int32 ValidateReativar(FUNCAO item, USUARIO usuario);
        List<FUNCAO> GetAllItens();
        FUNCAO GetItemById(Int32 id);
        List<FUNCAO> GetAllItensAdm();
    }
}
