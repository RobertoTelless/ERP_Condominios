using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class UFRepository : RepositoryBase<UF>, IUFRepository
    {
        public List<UF> GetAllItens()
        {
            List<UF> lista = new List<UF>();
            IQueryable<UF> query = Db.UF;
            query = query.OrderBy(a => a.UF_NM_NOME);
            lista = query.ToList<UF>();
            return lista;
        }

        public UF GetItemById(Int32 id)
        {
            IQueryable<UF> query = Db.UF.Where(p => p.UF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public UF GetItemBySigla(String sigla)
        {
            IQueryable<UF> query = Db.UF.Where(p => p.UF_SG_SIGLA == sigla);
            return query.FirstOrDefault();
        }

    }
}
