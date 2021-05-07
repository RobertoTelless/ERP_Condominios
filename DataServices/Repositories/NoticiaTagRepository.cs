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
    public class NoticiaTagRepository : RepositoryBase<NOTICIA_TAG>, INoticiaTagRepository
    {
        public List<NOTICIA_TAG> GetAllItens()
        {
            return Db.NOTICIA_TAG.ToList();
        }

        public NOTICIA_TAG GetItemById(Int32 id)
        {
            IQueryable<NOTICIA_TAG> query = Db.NOTICIA_TAG.Where(p => p.NOTA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 