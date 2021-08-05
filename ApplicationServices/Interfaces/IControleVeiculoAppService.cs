using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IControleVeiculoAppService : IAppServiceBase<CONTROLE_VEICULO>
    {
        Int32 ValidateCreate(CONTROLE_VEICULO item, USUARIO usuario);
        Int32 ValidateEdit(CONTROLE_VEICULO item, CONTROLE_VEICULO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CONTROLE_VEICULO item, CONTROLE_VEICULO itemAntes);
        Int32 ValidateDelete(CONTROLE_VEICULO item, USUARIO usuario);
        Int32 ValidateReativar(CONTROLE_VEICULO item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, CONTROLE_VEICULO contVeiculo, String template);

        CONTROLE_VEICULO GetItemById(Int32 id);
        List<CONTROLE_VEICULO> GetItemByData(DateTime data, Int32 idAss);
        List<CONTROLE_VEICULO> GetAllItens(Int32 idAss);
        List<CONTROLE_VEICULO> GetAllItensAdm(Int32 idAss);
        List<CONTROLE_VEICULO> GetByUnidade(Int32 idUnid);
        Int32 ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, DateTime? dataEntrada, DateTime? dataSaida, Int32 idAss, out List<CONTROLE_VEICULO> objeto);

        List<TIPO_VEICULO> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
