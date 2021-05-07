using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaBancariaService : IServiceBase<CONTA_BANCO>
    {
        Int32 Create(CONTA_BANCO perfil, LOG log);
        Int32 Create(CONTA_BANCO perfil);
        Int32 Edit(CONTA_BANCO perfil, LOG log);
        Int32 Edit(CONTA_BANCO perfil);
        Int32 Delete(CONTA_BANCO perfil, LOG log);
        CONTA_BANCO CheckExist(CONTA_BANCO conta);
        CONTA_BANCO GetItemById(Int32 id);
        CONTA_BANCO GetContaPadrao();
        List<CONTA_BANCO> GetAllItens();
        List<CONTA_BANCO> GetAllItensAdm();
        List<TIPO_CONTA> GetAllTipos();
        Decimal GetTotalContas();
        CONTA_BANCO_CONTATO GetContatoById(Int32 id);
        CONTA_BANCO_LANCAMENTO GetLancamentoById(Int32 id);
        Int32 EditContato(CONTA_BANCO_CONTATO item);
        Int32 CreateContato(CONTA_BANCO_CONTATO item);
        Int32 EditLancamento(CONTA_BANCO_LANCAMENTO item);
        Int32 CreateLancamento(CONTA_BANCO_LANCAMENTO item, CONTA_BANCO conta);
        Decimal GetTotalReceita(Int32 conta);
        Decimal GetTotalDespesa(Int32 conta);
        Decimal GetTotalReceitaMes(Int32 conta, Int32 mes);
        Decimal GetTotalDespesaMes(Int32 conta, Int32 mes);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosMes(Int32 conta, Int32 mes);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosDia(Int32 conta, DateTime data);
        List<CONTA_BANCO_LANCAMENTO> GetLancamentosFaixa(Int32 conta, DateTime inicio, DateTime final);
    }
}
