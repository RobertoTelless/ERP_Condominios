using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaPatrimonioAppService : IAppServiceBase<CATEGORIA_PATRIMONIO>
    {
        Int32 ValidateCreate(CATEGORIA_PATRIMONIO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_PATRIMONIO item, CATEGORIA_PATRIMONIO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_PATRIMONIO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_PATRIMONIO item, USUARIO usuario);
        List<CATEGORIA_PATRIMONIO> GetAllItens();
        CATEGORIA_PATRIMONIO GetItemById(Int32 id);
        List<CATEGORIA_PATRIMONIO> GetAllItensAdm();
    }
}
