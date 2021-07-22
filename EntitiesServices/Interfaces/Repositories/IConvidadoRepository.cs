using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConvidadoRepository : IRepositoryBase<CONVIDADO>
    {
        List<CONVIDADO> GetAllItens(Int32 idAss);
        CONVIDADO GetItemById(Int32 id);
    }
}
