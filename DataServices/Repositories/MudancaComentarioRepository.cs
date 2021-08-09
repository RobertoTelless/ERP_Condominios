using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class MudancaComentarioRepository : RepositoryBase<SOLICITACAO_MUDANCA_COMENTARIO>, IMudancaComentarioRepository
    {
        public List<SOLICITACAO_MUDANCA_COMENTARIO> GetAllItens()
        {
            return Db.SOLICITACAO_MUDANCA_COMENTARIO.ToList();
        }

        public SOLICITACAO_MUDANCA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<SOLICITACAO_MUDANCA_COMENTARIO> query = Db.SOLICITACAO_MUDANCA_COMENTARIO.Where(p => p.SMCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 