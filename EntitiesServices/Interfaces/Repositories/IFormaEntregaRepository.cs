using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaEntregaRepository : IRepositoryBase<FORMA_ENTREGA>
    {
        List<FORMA_ENTREGA> GetAllItens(Int32 idAss);
        FORMA_ENTREGA GetItemById(Int32 id);
    }
}
