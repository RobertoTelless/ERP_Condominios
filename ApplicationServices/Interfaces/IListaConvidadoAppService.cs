using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IListaConvidadoAppService : IAppServiceBase<LISTA_CONVIDADO>
    {
        Int32 ValidateCreate(LISTA_CONVIDADO item, USUARIO usuario);
        Int32 ValidateEdit(LISTA_CONVIDADO item, LISTA_CONVIDADO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(LISTA_CONVIDADO item, LISTA_CONVIDADO itemAntes);
        Int32 ValidateDelete(LISTA_CONVIDADO item, USUARIO usuario);
        Int32 ValidateReativar(LISTA_CONVIDADO item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, LISTA_CONVIDADO veiculo, String template);

        LISTA_CONVIDADO CheckExist(LISTA_CONVIDADO item, Int32 idAss);
        LISTA_CONVIDADO GetItemById(Int32 id);
        List<LISTA_CONVIDADO> GetAllItens(Int32 idAss);
        List<LISTA_CONVIDADO> GetAllItensAdm(Int32 idAss);
        List<LISTA_CONVIDADO> GetByUnidade(Int32 idUnid);
        Int32 ExecuteFilter(String nome, DateTime? data, Int32? unid, Int32? reserva, Int32 idAss, out List<LISTA_CONVIDADO> objeto);

        List<RESERVA> GetAllReservas(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

        CONVIDADO GetConvidadoById(Int32 id);
        Int32 ValidateEditConvidado(CONVIDADO item);
        Int32 ValidateCreateConvidado(CONVIDADO item);
    }
}
