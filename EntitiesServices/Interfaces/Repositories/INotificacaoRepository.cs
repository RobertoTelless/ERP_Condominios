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
        List<NOTIFICACAO> GetAllItens(Int32 idAss);
        List<NOTIFICACAO> GetAllItensAdm(Int32 idAss);
        List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss);
        List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto, Int32 idAss);
    }
}
