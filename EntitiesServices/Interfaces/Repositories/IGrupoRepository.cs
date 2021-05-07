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
        GRUPO CheckExist(GRUPO item);
        List<GRUPO> GetAllItens();
        GRUPO GetItemById(Int32 id);
        List<GRUPO> GetAllItensAdm();
    }
}
