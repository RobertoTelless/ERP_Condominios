using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEntradaSaidaRepository : IRepositoryBase<ENTRADA_SAIDA>
    {
        ENTRADA_SAIDA GetItemById(Int32 id);
        List<ENTRADA_SAIDA> GetAllItens(Int32 idAss);
        List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss);
        List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid);
        List<ENTRADA_SAIDA> GetByData(DateTime data, Int32 idAss);
        List<ENTRADA_SAIDA> ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada, DateTime? dataSaida, Int32? status, Int32 idAss);
    }
}
