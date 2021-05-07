using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TarefaNotificacaoRepository : RepositoryBase<TAREFA_NOTIFICACAO>, ITarefaNotificacaoRepository
    {
        public List<TAREFA_NOTIFICACAO> GetAllItens()
        {
            return Db.TAREFA_NOTIFICACAO.ToList();
        }

        public TAREFA_NOTIFICACAO GetItemById(Int32 id)
        {
            IQueryable<TAREFA_NOTIFICACAO> query = Db.TAREFA_NOTIFICACAO.Where(p => p.TANO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
