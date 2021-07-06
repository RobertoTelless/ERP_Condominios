using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoAmbienteRepository : IRepositoryBase<TIPO_AMBIENTE>
    {
        List<TIPO_AMBIENTE> GetAllItens(Int32 idAss);
        TIPO_AMBIENTE GetItemById(Int32 id);
        List<TIPO_AMBIENTE> GetAllItensAdm(Int32 idAss);
    }
}
