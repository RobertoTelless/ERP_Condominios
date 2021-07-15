using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISubgrupoRepository : IRepositoryBase<SUBGRUPO>
    {
        SUBGRUPO CheckExist(SUBGRUPO item, Int32 idAss);
        List<SUBGRUPO> GetAllItens(Int32 idAss);
        SUBGRUPO GetItemById(Int32 id);
        List<SUBGRUPO> GetAllItensAdm(Int32 idAss);
    }
}
