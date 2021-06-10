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
    public class TipoVagaRepository : RepositoryBase<TIPO_VAGA>, ITipoVagaRepository
    {
        public TIPO_VAGA GetItemById(Int32 id)
        {
            IQueryable<TIPO_VAGA> query = Db.TIPO_VAGA;
            query = query.Where(p => p.TIVA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_VAGA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_VAGA> query = Db.TIPO_VAGA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_VAGA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_VAGA> query = Db.TIPO_VAGA.Where(p => p.TIVA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 