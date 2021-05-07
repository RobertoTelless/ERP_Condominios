using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFuncaoRepository : IRepositoryBase<FUNCAO>
    {
        List<FUNCAO> GetAllItens();
        FUNCAO GetItemById(Int32 id);
        List<FUNCAO> GetAllItensAdm();
    }
}
