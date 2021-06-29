using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVagaAppService : IAppServiceBase<VAGA>
    {
        Int32 ValidateCreate(VAGA item, USUARIO usuario);
        Int32 ValidateEdit(VAGA item, VAGA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(VAGA item, VAGA itemAntes);
        Int32 ValidateDelete(VAGA item, USUARIO usuario);
        Int32 ValidateReativar(VAGA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, VAGA vaga, String template);
        Int32 ValidateAtribuicao(VAGA item, VAGA itemAntes, USUARIO usuario);

        VAGA CheckExist(VAGA item, Int32 idAss);
        VAGA GetItemById(Int32 id);
        List<VAGA> GetAllItens(Int32 idAss);
        List<VAGA> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(String numero, String andar, Int32? unid, Int32? idTipo, Int32 idAss, out List<VAGA> objeto);
        List<TIPO_VAGA> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<VEICULO> GetVeiculosUnidade(Int32 idUnid);
    }
}
