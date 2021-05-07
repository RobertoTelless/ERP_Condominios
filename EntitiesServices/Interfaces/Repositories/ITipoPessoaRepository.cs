using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoPessoaRepository : IRepositoryBase<TIPO_PESSOA>
    {
        List<TIPO_PESSOA> GetAllItens();
        TIPO_PESSOA GetItemById(Int32 id);
        List<TIPO_PESSOA> GetAllItensAdm();
    }
}
