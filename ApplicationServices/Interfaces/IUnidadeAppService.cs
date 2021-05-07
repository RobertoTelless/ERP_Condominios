using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUnidadeAppService : IAppServiceBase<UNIDADE>
    {
        Int32 ValidateCreate(UNIDADE item, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes);
        Int32 ValidateDelete(UNIDADE item, USUARIO usuario);
        Int32 ValidateReativar(UNIDADE item, USUARIO usuario);
        List<UNIDADE> GetAllItens();
        List<UNIDADE> GetAllItensAdm();
        UNIDADE GetItemById(Int32 id);
    }
}
