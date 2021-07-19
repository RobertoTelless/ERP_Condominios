using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoMaterialRepository : IRepositoryBase<TIPO_MATERIAL>
    {
        List<TIPO_MATERIAL> GetAllItens(Int32 idAss);
        TIPO_MATERIAL GetItemById(Int32 id);
        List<TIPO_MATERIAL> GetAllItensAdm(Int32 idAss);
    }
}
