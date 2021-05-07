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
        List<NOTIFICACAO> GetAllItens();
        List<NOTIFICACAO> GetAllItensAdm();
        List<NOTIFICACAO> GetAllItensUser(Int32 id);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id);
        List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto);
        NOTIFICACAO_ANEXO GetAnexoById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllCategorias();
    }
}
