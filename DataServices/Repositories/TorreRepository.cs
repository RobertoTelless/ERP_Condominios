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
    public class TorreRepository : RepositoryBase<TORRE>, ITorreRepository
    {
        public TORRE GetItemById(Int32 id)
        {
            IQueryable<TORRE> query = Db.TORRE;
            query = query.Where(p => p.TORR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TORRE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TORRE> query = Db.TORRE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TORRE> GetAllItens(Int32 idAss)
        {
            IQueryable<TORRE> query = Db.TORRE.Where(p => p.TORR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 