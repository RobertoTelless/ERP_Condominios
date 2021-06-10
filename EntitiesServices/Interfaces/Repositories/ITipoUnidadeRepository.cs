using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoUnidadeRepository : IRepositoryBase<TIPO_UNIDADE>
    {
        List<TIPO_UNIDADE> GetAllItens(Int32 idAss);
        TIPO_UNIDADE GetItemById(Int32 id);
        List<TIPO_UNIDADE> GetAllItensAdm(Int32 idAss);
    }
}
