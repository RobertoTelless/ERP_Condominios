using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAmbienteChaveRepository : IRepositoryBase<AMBIENTE_CHAVE>
    {
        List<AMBIENTE_CHAVE> GetAllItens(Int32 idAss);
        AMBIENTE_CHAVE GetItemById(Int32 id);
    }
}
