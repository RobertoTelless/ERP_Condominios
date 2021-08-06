using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICorpoDiretivoRepository : IRepositoryBase<CORPO_DIRETIVO>
    {
        CORPO_DIRETIVO CheckExist(CORPO_DIRETIVO item, Int32 idAss);
        List<CORPO_DIRETIVO> GetAllItens(Int32 idAss);
        CORPO_DIRETIVO GetItemById(Int32 id);
        List<CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss);
    }
}
