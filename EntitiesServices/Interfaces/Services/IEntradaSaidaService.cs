using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEntradaSaidaService : IServiceBase<ENTRADA_SAIDA>
    {
        Int32 Create(ENTRADA_SAIDA item, LOG log);
        Int32 Create(ENTRADA_SAIDA item);
        Int32 Edit(ENTRADA_SAIDA item, LOG log);
        Int32 Edit(ENTRADA_SAIDA item);
        Int32 Delete(ENTRADA_SAIDA item, LOG log);

        ENTRADA_SAIDA GetItemById(Int32 id);
        List<ENTRADA_SAIDA> GetAllItens(Int32 idAss);
        List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss);
        List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid);
        List<ENTRADA_SAIDA> GetByData(DateTime data, Int32 idAss);
        List<ENTRADA_SAIDA> ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada, DateTime? dataSaida, Int32? status, Int32 idAss);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<AUTORIZACAO_ACESSO> GetAllAutorizacoes(Int32 idAss);
        List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss);
    }
}
