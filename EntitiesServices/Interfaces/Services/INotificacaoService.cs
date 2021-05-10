using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface INotificacaoService : IServiceBase<NOTIFICACAO>
    {
        Int32 Create(NOTIFICACAO item, LOG log);
        Int32 Create(NOTIFICACAO item);
        Int32 Edit(NOTIFICACAO item, LOG log);
        Int32 Edit(NOTIFICACAO item);
        Int32 Delete(NOTIFICACAO item, LOG log);

        NOTIFICACAO GetItemById(Int32 id);
        List<NOTIFICACAO> GetAllItens(Int32 idAss);
        List<NOTIFICACAO> GetAllItensAdm(Int32 idAss);
        List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss);
        List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto, Int32 idAss);
        NOTIFICACAO_ANEXO GetAnexoById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllCategorias(Int32 idAss);
    }
}
