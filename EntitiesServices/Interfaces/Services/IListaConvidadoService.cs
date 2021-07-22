using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IListaConvidadoService : IServiceBase<LISTA_CONVIDADO>
    {
        Int32 Create(LISTA_CONVIDADO item, LOG log);
        Int32 Create(LISTA_CONVIDADO item);
        Int32 Edit(LISTA_CONVIDADO item, LOG log);
        Int32 Edit(LISTA_CONVIDADO item);
        Int32 Delete(LISTA_CONVIDADO item, LOG log);

        LISTA_CONVIDADO CheckExist(LISTA_CONVIDADO item, Int32 idAss);
        LISTA_CONVIDADO GetItemById(Int32 id);
        List<LISTA_CONVIDADO> GetAllItens(Int32 idAss);
        List<LISTA_CONVIDADO> GetAllItensAdm(Int32 idAss);
        List<LISTA_CONVIDADO> GetByUnidade(Int32 idUnid);
        List<LISTA_CONVIDADO> ExecuteFilter(String nome, DateTime? data, Int32? unid, Int32? reserva, Int32 idAss);

        List<RESERVA> GetAllReservas(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

        CONVIDADO GetConvidadoById(Int32 id);
        Int32 EditConvidado(CONVIDADO item);
        Int32 CreateConvidado(CONVIDADO item);
    }
}
