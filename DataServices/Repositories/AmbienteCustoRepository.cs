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
    public class AmbienteCustoRepository : RepositoryBase<AMBIENTE_CUSTO>, IAmbienteCustoRepository
    {
        public AMBIENTE_CUSTO GetItemById(Int32 id)
        {
            IQueryable<AMBIENTE_CUSTO> query = Db.AMBIENTE_CUSTO;
            query = query.Where(p => p.AMCU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<AMBIENTE_CUSTO> GetAllItens(Int32 idAss)
        {
            IQueryable<AMBIENTE_CUSTO> query = Db.AMBIENTE_CUSTO.Where(p => p.AMCU_NM_ATIVO == 1);
            return query.ToList();
        }
    }
}
 