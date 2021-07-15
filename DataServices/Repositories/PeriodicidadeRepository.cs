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
    public class PeriodicidadeRepository : RepositoryBase<PERIODICIDADE>, IPeriodicidadeRepository
    {
        public PERIODICIDADE GetItemById(Int32 id)
        {
            IQueryable<PERIODICIDADE> query = Db.PERIODICIDADE;
            query = query.Where(p => p.PERI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PERIODICIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PERIODICIDADE> query = Db.PERIODICIDADE;
            return query.ToList();
        }

        public List<PERIODICIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<PERIODICIDADE> query = Db.PERIODICIDADE.Where(p => p.PERI_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 