using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoCondominioRepository : IRepositoryBase<TIPO_CONDOMINIO>
    {
        List<TIPO_CONDOMINIO> GetAllItens();
        TIPO_CONDOMINIO GetItemById(Int32 id);
        List<TIPO_CONDOMINIO> GetAllItensAdm();
    }
}
