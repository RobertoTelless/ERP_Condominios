using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAmbienteCustoRepository : IRepositoryBase<AMBIENTE_CUSTO>
    {
        List<AMBIENTE_CUSTO> GetAllItens(Int32 idAss);
        AMBIENTE_CUSTO GetItemById(Int32 id);
    }
}
