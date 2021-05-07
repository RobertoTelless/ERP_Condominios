using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUnidadeRepository : IRepositoryBase<UNIDADE>
    {
        List<UNIDADE> GetAllItens();
        UNIDADE GetItemById(Int32 id);
        List<UNIDADE> GetAllItensAdm();
    }
}
