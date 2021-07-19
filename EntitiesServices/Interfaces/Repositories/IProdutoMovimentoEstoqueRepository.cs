using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoMovimentoEstoqueRepository : IRepositoryBase<MOVIMENTO_ESTOQUE_PRODUTO>
    {
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens();
        MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id);
    }
}
