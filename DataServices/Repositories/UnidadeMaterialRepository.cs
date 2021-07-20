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
    public class UnidadeMaterialRepository : RepositoryBase<UNIDADE_MATERIAL>, IUnidadeMaterialRepository
    {
        public UNIDADE_MATERIAL GetItemById(Int32 id)
        {
            IQueryable<UNIDADE_MATERIAL> query = Db.UNIDADE_MATERIAL;
            query = query.Where(p => p.UNMA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<UNIDADE_MATERIAL> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<UNIDADE_MATERIAL> query = Db.UNIDADE_MATERIAL;
            return query.ToList();
        }

        public List<UNIDADE_MATERIAL> GetAllItens(Int32 idAss)
        {
            IQueryable<UNIDADE_MATERIAL> query = Db.UNIDADE_MATERIAL.Where(p => p.UNMA_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 