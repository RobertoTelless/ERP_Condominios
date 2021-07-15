using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPeriodicidadeRepository : IRepositoryBase<PERIODICIDADE>
    {
        List<PERIODICIDADE> GetAllItens(Int32 idAss);
        PERIODICIDADE GetItemById(Int32 id);
        List<PERIODICIDADE> GetAllItensAdm(Int32 idAss);
    }
}
