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
    public class EncomendaComentarioRepository : RepositoryBase<ENCOMENDA_COMENTARIO>, IEncomendaComentarioRepository
    {
        public List<ENCOMENDA_COMENTARIO> GetAllItens()
        {
            return Db.ENCOMENDA_COMENTARIO.ToList();
        }

        public ENCOMENDA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<ENCOMENDA_COMENTARIO> query = Db.ENCOMENDA_COMENTARIO.Where(p => p.ECOM_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 