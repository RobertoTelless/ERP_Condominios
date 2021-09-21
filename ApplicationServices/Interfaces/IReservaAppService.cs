using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IReservaAppService : IAppServiceBase<RESERVA>
    {
        Int32 ValidateCreate(RESERVA item, USUARIO usuario);
        Int32 ValidateEdit(RESERVA item, RESERVA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(RESERVA item, RESERVA itemAntes);
        Int32 ValidateDelete(RESERVA item, USUARIO usuario);
        Int32 ValidateReativar(RESERVA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, RESERVA entrada, String template);

        RESERVA CheckExist(RESERVA item, Int32 idAss);
        RESERVA GetItemById(Int32 id);
        List<RESERVA> GetAllItens(Int32 idAss);
        List<RESERVA> GetAllItensAdm(Int32 idAss);
        List<RESERVA> GetByUnidade(Int32 idUnid);
        List<RESERVA> GetItemByData(DateTime data, Int32 idAss);
        RESERVA_COMENTARIO GetComentarioById(Int32 id);
        RESERVA_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32? status, Int32 idAss, out List<RESERVA> objeto);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<FINALIDADE_RESERVA> GetAllFinalidades(Int32 idAss);
        List<AMBIENTE> GetAllAmbientes(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
    }
}
