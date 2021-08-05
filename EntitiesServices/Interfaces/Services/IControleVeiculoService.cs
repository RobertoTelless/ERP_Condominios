using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IControleVeiculoService : IServiceBase<CONTROLE_VEICULO>
    {
        Int32 Create(CONTROLE_VEICULO item, LOG log);
        Int32 Create(CONTROLE_VEICULO item);
        Int32 Edit(CONTROLE_VEICULO item, LOG log);
        Int32 Edit(CONTROLE_VEICULO item);
        Int32 Delete(CONTROLE_VEICULO item, LOG log);

        CONTROLE_VEICULO GetItemById(Int32 id);
        List<CONTROLE_VEICULO> GetAllItens(Int32 idAss);
        List<CONTROLE_VEICULO> GetAllItensAdm(Int32 idAss);
        List<CONTROLE_VEICULO> GetByUnidade(Int32 idUnid);
        List<CONTROLE_VEICULO> GetByData(DateTime data, Int32 idAss);
        List<CONTROLE_VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, DateTime? dataEntrada, DateTime? dataSaida, Int32 idAss);

        List<TIPO_VEICULO> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
