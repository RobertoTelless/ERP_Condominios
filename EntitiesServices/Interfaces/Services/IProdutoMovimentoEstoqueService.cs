using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IProdutoMovimentoEstoqueService : IServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>
    {
        Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO perfil, LOG log);
        Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO perfil);
        //Int32 Edit(PRODUTO_MOVIMENTO_ESTOQUE perfil, LOG log);
        //Int32 Edit(PRODUTO_MOVIMENTO_ESTOQUE perfil);
        //Int32 Delete(PRODUTO_MOVIMENTO_ESTOQUE perfil, LOG log);
        //PRODUTO_MOVIMENTO_ESTOQUE GetItemById(Int32 id);
        //List<PRODUTO_MOVIMENTO_ESTOQUE> GetAllItens();
        //List<PRODUTO_MOVIMENTO_ESTOQUE> GetAllItensAdm();
    }
}
