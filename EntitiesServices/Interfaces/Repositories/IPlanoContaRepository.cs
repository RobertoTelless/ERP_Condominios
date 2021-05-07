using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPlanoContaRepository : IRepositoryBase<PLANO_CONTA>
    {
        PLANO_CONTA CheckExist(PLANO_CONTA item);
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItens();
        List<PLANO_CONTA> GetAllItensAdm();
    }
}
