using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUFRepository : IRepositoryBase<UF>
    {
        List<UF> GetAllItens();
        UF GetItemById(Int32 id);
        UF GetItemBySigla(String sigla);
    }
}
