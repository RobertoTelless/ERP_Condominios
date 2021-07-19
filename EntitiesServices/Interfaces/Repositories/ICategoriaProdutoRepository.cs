using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaProdutoRepository : IRepositoryBase<CATEGORIA_PRODUTO>
    {
        CATEGORIA_PRODUTO CheckExist(CATEGORIA_PRODUTO item, Int32 idAss);
        List<CATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        CATEGORIA_PRODUTO GetItemById(Int32 id);
        List<CATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
    }
}
