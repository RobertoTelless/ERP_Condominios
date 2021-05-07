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
    public class NoticiaAvaliacaoRepository : RepositoryBase<NOTICIA_AVALIACAO>, INoticiaAvaliacaoRepository
    {
        public List<NOTICIA_AVALIACAO> GetAllItens()
        {
            return Db.NOTICIA_AVALIACAO.ToList();
        }

        public NOTICIA_AVALIACAO GetItemById(Int32 id)
        {
            IQueryable<NOTICIA_AVALIACAO> query = Db.NOTICIA_AVALIACAO.Where(p => p.NOAL_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 