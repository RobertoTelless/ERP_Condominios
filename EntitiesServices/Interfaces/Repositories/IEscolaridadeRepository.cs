using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEscolaridadeRepository : IRepositoryBase<ESCOLARIDADE>
    {
        List<ESCOLARIDADE> GetAllItens();
        ESCOLARIDADE GetItemById(Int32 id);
        List<ESCOLARIDADE> GetAllItensAdm();
    }
}
