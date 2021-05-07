using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INotificacaoRepository : IRepositoryBase<NOTIFICACAO>
    {
        NOTIFICACAO GetItemById(Int32 id);
        List<NOTIFICACAO> GetAllItens();
        List<NOTIFICACAO> GetAllItensAdm();
        List<NOTIFICACAO> GetAllItensUser(Int32 id);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id);
        List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto);
    }
}
