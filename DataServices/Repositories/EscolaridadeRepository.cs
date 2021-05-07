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
    public class EscolaridadeRepository : RepositoryBase<ESCOLARIDADE>, IEscolaridadeRepository
    {
        public ESCOLARIDADE GetItemById(Int32 id)
        {
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE;
            query = query.Where(p => p.ESCO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ESCOLARIDADE> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ESCOLARIDADE> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE.Where(p => p.ESCO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 