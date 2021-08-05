using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IControleVeiculoRepository : IRepositoryBase<CONTROLE_VEICULO>
    {
        CONTROLE_VEICULO GetItemById(Int32 id);
        List<CONTROLE_VEICULO> GetAllItens(Int32 idAss);
        List<CONTROLE_VEICULO> GetAllItensAdm(Int32 idAss);
        List<CONTROLE_VEICULO> GetByUnidade(Int32 idUnid);
        List<CONTROLE_VEICULO> GetByData(DateTime data, Int32 idAss);
        List<CONTROLE_VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, DateTime? dataEntrada, DateTime? dataSaida, Int32 idAss);
    }
}
