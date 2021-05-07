using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITarefaNotificacaoRepository : IRepositoryBase<TAREFA_NOTIFICACAO>
    {
        List<TAREFA_NOTIFICACAO> GetAllItens();
        TAREFA_NOTIFICACAO GetItemById(Int32 id);
    }
}
