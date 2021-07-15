using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoRepository : IRepositoryBase<GRUPO>
    {
        GRUPO CheckExist(GRUPO item, Int32 idAss);
        List<GRUPO> GetAllItens(Int32 idAss);
        GRUPO GetItemById(Int32 id);
        List<GRUPO> GetAllItensAdm(Int32 idAss);
    }
}
