using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVeiculoAppService : IAppServiceBase<VEICULO>
    {
        Int32 ValidateCreate(VEICULO item, USUARIO usuario);
        Int32 ValidateEdit(VEICULO item, VEICULO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(VEICULO item, VEICULO itemAntes);
        Int32 ValidateDelete(VEICULO item, USUARIO usuario);
        Int32 ValidateReativar(VEICULO item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, VEICULO veiculo, String template);

        VEICULO CheckExist(VEICULO item, Int32 idAss);
        VEICULO GetItemById(Int32 id);
        List<VEICULO> GetAllItens(Int32 idAss);
        List<VEICULO> GetAllItensAdm(Int32 idAss);
        List<VEICULO> GetByUnidade(Int32 idUnid);
        Int32 ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, Int32? vaga, Int32 idAss, out List<VEICULO> objeto);

        VEICULO_ANEXO GetAnexoById(Int32 id);
        List<TIPO_VEICULO> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<VAGA> GetAllVagas(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
