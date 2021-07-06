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
    public class TipoAmbienteRepository : RepositoryBase<TIPO_AMBIENTE>, ITipoAmbienteRepository
    {
        public TIPO_AMBIENTE GetItemById(Int32 id)
        {
            IQueryable<TIPO_AMBIENTE> query = Db.TIPO_AMBIENTE;
            query = query.Where(p => p.TIAM_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_AMBIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_AMBIENTE> query = Db.TIPO_AMBIENTE;
            return query.ToList();
        }

        public List<TIPO_AMBIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_AMBIENTE> query = Db.TIPO_AMBIENTE.Where(p => p.TIAM_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 