using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrauParentescoRepository : IRepositoryBase<GRAU_PARENTESCO>
    {
        List<GRAU_PARENTESCO> GetAllItens(Int32 idAss);
        GRAU_PARENTESCO GetItemById(Int32 id);
        List<GRAU_PARENTESCO> GetAllItensAdm(Int32 idAss);
    }
}
