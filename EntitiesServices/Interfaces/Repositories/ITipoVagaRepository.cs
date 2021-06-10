using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoVagaRepository : IRepositoryBase<TIPO_VAGA>
    {
        List<TIPO_VAGA> GetAllItens(Int32 idAss);
        TIPO_VAGA GetItemById(Int32 id);
        List<TIPO_VAGA> GetAllItensAdm(Int32 idAss);
    }
}
