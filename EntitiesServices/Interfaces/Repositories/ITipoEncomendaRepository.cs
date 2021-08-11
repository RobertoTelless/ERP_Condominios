using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoEncomendaRepository : IRepositoryBase<TIPO_ENCOMENDA>
    {
        List<TIPO_ENCOMENDA> GetAllItens(Int32 idAss);
        TIPO_ENCOMENDA GetItemById(Int32 id);
        List<TIPO_ENCOMENDA> GetAllItensAdm(Int32 idAss);
    }
}
