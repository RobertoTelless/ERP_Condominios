using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoTarefaRepository : RepositoryBase<TIPO_TAREFA>, ITipoTarefaRepository
    {
        public TIPO_TAREFA GetItemById(Int32 id)
        {
            IQueryable<TIPO_TAREFA> query = Db.TIPO_TAREFA;
            query = query.Where(p => p.TITR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_TAREFA> GetAllItens()
        {
            IQueryable<TIPO_TAREFA> query = Db.TIPO_TAREFA;
            return query.ToList();
        }
    }
}
