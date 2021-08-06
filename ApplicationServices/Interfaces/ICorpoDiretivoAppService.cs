using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICorpoDiretivoAppService : IAppServiceBase<CORPO_DIRETIVO>
    {
        Int32 ValidateCreate(CORPO_DIRETIVO item, USUARIO usuario);
        Int32 ValidateEdit(CORPO_DIRETIVO item, CORPO_DIRETIVO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CORPO_DIRETIVO item, CORPO_DIRETIVO itemAntes);
        Int32 ValidateDelete(CORPO_DIRETIVO item, USUARIO usuario);
        Int32 ValidateReativar(CORPO_DIRETIVO item, USUARIO usuario);

        CORPO_DIRETIVO CheckExist(CORPO_DIRETIVO obj, Int32 idAss);
        List<CORPO_DIRETIVO> GetAllItens(Int32 idAss);
        List<CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss);
        CORPO_DIRETIVO GetItemById(Int32 id);

        List<FUNCAO_CORPO_DIRETIVO> GetAllFuncoes(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

    }
}
