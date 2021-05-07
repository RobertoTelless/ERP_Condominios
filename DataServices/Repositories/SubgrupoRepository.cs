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
    public class SubgrupoRepository : RepositoryBase<SUBGRUPO>, ISubgrupoRepository
    {
        public SUBGRUPO CheckExist(SUBGRUPO conta)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<SUBGRUPO> query = Db.SUBGRUPO;
            query = query.Where(p => p.SUBG_NR_NUMERO == conta.SUBG_NR_NUMERO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public SUBGRUPO GetItemById(Int32 id)
        {
            IQueryable<SUBGRUPO> query = Db.SUBGRUPO;
            query = query.Where(p => p.SUBG_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SUBGRUPO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<SUBGRUPO> query = Db.SUBGRUPO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SUBGRUPO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<SUBGRUPO> query = Db.SUBGRUPO.Where(p => p.SUBG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 