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
    public class ConvidadoRepository : RepositoryBase<CONVIDADO>, IConvidadoRepository
    {
        public List<CONVIDADO> GetAllItens(Int32 idAss)
        {
            return Db.CONVIDADO.ToList();
        }

        public CONVIDADO GetItemById(Int32 id)
        {
            IQueryable<CONVIDADO> query = Db.CONVIDADO.Where(p => p.CONV_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 