using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUnidadeMaterialRepository : IRepositoryBase<UNIDADE_MATERIAL>
    {
        List<UNIDADE_MATERIAL> GetAllItens(Int32 idAss);
        UNIDADE_MATERIAL GetItemById(Int32 id);
        List<UNIDADE_MATERIAL> GetAllItensAdm(Int32 idAss);
    }
}
