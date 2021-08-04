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
    public class AmbienteChaveRepository : RepositoryBase<AMBIENTE_CHAVE>, IAmbienteChaveRepository
    {
        public AMBIENTE_CHAVE GetItemById(Int32 id)
        {
            IQueryable<AMBIENTE_CHAVE> query = Db.AMBIENTE_CHAVE;
            query = query.Where(p => p.AMCH_CD_ID == id);
            query = query.Include(p => p.AMBIENTE);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<AMBIENTE_CHAVE> GetAllItens(Int32 idAss)
        {
            IQueryable<AMBIENTE_CHAVE> query = Db.AMBIENTE_CHAVE.Where(p => p.AMCH_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 