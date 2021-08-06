using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICorpoDiretivoService : IServiceBase<CORPO_DIRETIVO>
    {
        Int32 Create(CORPO_DIRETIVO item, LOG log);
        Int32 Create(CORPO_DIRETIVO item);
        Int32 Edit(CORPO_DIRETIVO item, LOG log);
        Int32 Edit(CORPO_DIRETIVO item);
        Int32 Delete(CORPO_DIRETIVO item, LOG log);

        CORPO_DIRETIVO GetItemById(Int32 id);
        CORPO_DIRETIVO CheckExist(CORPO_DIRETIVO item, Int32 idAss);
        List<CORPO_DIRETIVO> GetAllItens(Int32 idAss);
        List<CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss);

        List<FUNCAO_CORPO_DIRETIVO> GetAllFuncoes(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
