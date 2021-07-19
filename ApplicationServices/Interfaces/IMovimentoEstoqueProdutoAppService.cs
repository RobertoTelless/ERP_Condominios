using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMovimentoEstoqueProdutoAppService : IAppServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>
    {
        Int32 ValidateEdit(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario);
        Int32 ValidateCreate(MOVIMENTO_ESTOQUE_PRODUTO mov, USUARIO usuario);
        Int32 ValidateCreateLeve(MOVIMENTO_ESTOQUE_PRODUTO mov, USUARIO usuario);
        Int32 ValidateCreateLista(List<MOVIMENTO_ESTOQUE_PRODUTO> lista);
        Int32 ValidateDelete(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario);
        Int32 ValidateReativar(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario);

        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss);
        MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id);
        MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada(Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida(Int32 idAss);
        Int32 ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? idAss, DateTime? dtMov, out List<MOVIMENTO_ESTOQUE_PRODUTO> objeto);
        Int32 ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? idAss, Int32? prod, out List<MOVIMENTO_ESTOQUE_PRODUTO> objeto);
    }
}
