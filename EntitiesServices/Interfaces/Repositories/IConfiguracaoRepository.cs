using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConfiguracaoRepository : IRepositoryBase<CONFIGURACAO>
    {
        CONFIGURACAO GetItemById(Int32 id);
        List<CONFIGURACAO> GetAllItems(Int32 idAss);
    }
}
