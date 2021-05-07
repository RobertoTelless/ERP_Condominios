using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFilialAppService : IAppServiceBase<FILIAL>
    {
        Int32 ValidateCreate(FILIAL perfil, USUARIO usuario);
        Int32 ValidateEdit(FILIAL perfil, FILIAL perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(FILIAL perfil, USUARIO usuario);
        Int32 ValidateReativar(FILIAL perfil, USUARIO usuario);
        List<FILIAL> GetAllItens();
        List<FILIAL> GetAllItensAdm();
        FILIAL GetItemById(Int32 id);
        FILIAL CheckExist(FILIAL filial);
        List<UF> GetAllUF();
    }
}
