using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFuncaoCorpoDiretivoRepository : IRepositoryBase<FUNCAO_CORPO_DIRETIVO>
    {
        List<FUNCAO_CORPO_DIRETIVO> GetAllItens(Int32 idAss);
        FUNCAO_CORPO_DIRETIVO GetItemById(Int32 id);
        List<FUNCAO_CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss);
    }
}
