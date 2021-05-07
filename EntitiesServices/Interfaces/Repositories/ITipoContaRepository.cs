using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoContaRepository : IRepositoryBase<TIPO_CONTA>
    {
        List<TIPO_CONTA> GetAllItens();
        TIPO_CONTA GetItemById(Int32 id);
    }
}
