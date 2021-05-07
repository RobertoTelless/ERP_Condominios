using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMatrizRepository : IRepositoryBase<MATRIZ>
    {
        MATRIZ GetItemById(Int32 id);
        List<MATRIZ> GetAllItens();
        List<MATRIZ> GetAllItensAdm();
    }
}
