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
    public class NoticiaComentarioRepository : RepositoryBase<NOTICIA_COMENTARIO>, INoticiaComentarioRepository
    {
        public List<NOTICIA_COMENTARIO> GetAllItens()
        {
            return Db.NOTICIA_COMENTARIO.ToList();
        }

        public NOTICIA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<NOTICIA_COMENTARIO> query = Db.NOTICIA_COMENTARIO.Where(p => p.NOCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 