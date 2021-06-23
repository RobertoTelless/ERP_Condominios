using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITorreRepository : IRepositoryBase<TORRE>
    {
        List<TORRE> GetAllItens(Int32 idAss);
        TORRE GetItemById(Int32 id);
        List<TORRE> GetAllItensAdm(Int32 idAss);
    }
}
