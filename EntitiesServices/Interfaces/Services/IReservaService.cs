using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IReservaService : IServiceBase<RESERVA>
    {
        Int32 Create(RESERVA item, LOG log);
        Int32 Create(RESERVA item);
        Int32 Edit(RESERVA item, LOG log);
        Int32 Edit(RESERVA item);
        Int32 Delete(RESERVA item, LOG log);

        RESERVA CheckExist(RESERVA item, Int32 idAss);
        RESERVA GetItemById(Int32 id);
        List<RESERVA> GetAllItens(Int32 idAss);
        List<RESERVA> GetAllItensAdm(Int32 idAss);
        List<RESERVA> GetByUnidade(Int32 idUnid);
        List<RESERVA> GetByData(DateTime data, Int32 idAss);
        List<RESERVA> ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32? status, Int32 idAss);
       
        RESERVA_COMENTARIO GetComentarioById(Int32 id);
        RESERVA_ANEXO GetAnexoById(Int32 id);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<FINALIDADE_RESERVA> GetAllFinalidades(Int32 idAss);
        List<AMBIENTE> GetAllAmbientes(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
    }
}
