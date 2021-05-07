using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFilialRepository : IRepositoryBase<FILIAL>
    {
        FILIAL CheckExist(FILIAL item);
        FILIAL GetItemById(Int32 id);
        List<FILIAL> GetAllItens();
        List<FILIAL> GetAllItensAdm();
    }
}
