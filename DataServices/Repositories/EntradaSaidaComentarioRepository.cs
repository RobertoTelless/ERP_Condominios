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
    public class EntradaSaidaComentarioRepository : RepositoryBase<ENTRADA_SAIDA_COMENTARIO>, IEntradaSaidaComentarioRepository
    {
        public List<ENTRADA_SAIDA_COMENTARIO> GetAllItens()
        {
            return Db.ENTRADA_SAIDA_COMENTARIO.ToList();
        }

        public ENTRADA_SAIDA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<ENTRADA_SAIDA_COMENTARIO> query = Db.ENTRADA_SAIDA_COMENTARIO.Where(p => p.ESCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 