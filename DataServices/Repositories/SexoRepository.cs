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
    public class SexoRepository : RepositoryBase<SEXO>, ISexoRepository
    {
        public SEXO GetItemById(Int32 id)
        {
            IQueryable<SEXO> query = Db.SEXO;
            query = query.Where(p => p.SEXO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SEXO> GetAllItensAdm()
        {
            IQueryable<SEXO> query = Db.SEXO;
            return query.ToList();
        }

        public List<SEXO> GetAllItens()
        {
            IQueryable<SEXO> query = Db.SEXO.Where(p => p.SEXO_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 