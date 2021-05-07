using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TarefaAnexoRepository : RepositoryBase<TAREFA_ANEXO>, ITarefaAnexoRepository
    {
        public List<TAREFA_ANEXO> GetAllItens()
        {
            return Db.TAREFA_ANEXO.ToList();
        }

        public TAREFA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<TAREFA_ANEXO> query = Db.TAREFA_ANEXO.Where(p => p.TAAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
