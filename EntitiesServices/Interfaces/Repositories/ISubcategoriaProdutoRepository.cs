using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISubcategoriaProdutoRepository : IRepositoryBase<SUBCATEGORIA_PRODUTO>
    {
        SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO item, Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        SUBCATEGORIA_PRODUTO GetItemById(Int32 id);
        List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
    }
}
