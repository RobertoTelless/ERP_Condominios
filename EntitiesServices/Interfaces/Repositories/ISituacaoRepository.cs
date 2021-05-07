using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISituacaoRepository : IRepositoryBase<SITUACAO>
    {
        List<SITUACAO> GetAllItens();
        SITUACAO GetItemById(Int32 id);
        List<SITUACAO> GetAllItensAdm();
    }
}
