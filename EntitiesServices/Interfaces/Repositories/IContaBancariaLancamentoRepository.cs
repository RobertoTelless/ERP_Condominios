using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaBancariaLancamentoRepository : IRepositoryBase<CONTA_BANCO_LANCAMENTO>
    {
        List<CONTA_BANCO_LANCAMENTO> GetAllItens(Int32 idConta);
        CONTA_BANCO_LANCAMENTO GetItemById(Int32 id);
        Decimal GetTotalReceita(Int32 conta);
        Decimal GetTotalDespesa(Int32 conta);
        Decimal GetTotalReceitaMes(Int32 conta, Int32 mes);
        Decimal GetTotalDespesaMes(Int32 conta, Int32 mes);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosMes(Int32 conta, Int32 mes);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosDia(Int32 conta, DateTime data);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosFaixa(Int32 conta, DateTime inicio, DateTime final);
        List<CONTA_BANCO_LANCAMENTO> ExecuteFilter(Int32 conta, DateTime? data, Int32? tipo, String desc);
    }
}

