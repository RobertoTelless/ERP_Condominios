using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoRepository : IRepositoryBase<PRODUTO>
    {
        PRODUTO CheckExist(PRODUTO item, Int32 idAss);
        PRODUTO CheckExist(String barcode, String codigo, Int32 idAss);
        PRODUTO GetByNome(String nome, Int32 idAss);
        PRODUTO GetItemById(Int32 id);
        List<PRODUTO> GetAllItens(Int32 idAss);
        List<PRODUTO> GetAllItensAdm(Int32 idAss);
        List<PRODUTO> GetPontoPedido(Int32 idAss);
        List<PRODUTO> GetEstoqueZerado(Int32 idAss);
        List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, String cod, Int32? idAss, Int32 ativo);
    }
}
