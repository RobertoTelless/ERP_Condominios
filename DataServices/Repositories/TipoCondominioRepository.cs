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
    public class TipoCondominioRepository : RepositoryBase<TIPO_CONDOMINIO>, ITipoCondominioRepository
    {
        public TIPO_CONDOMINIO GetItemById(Int32 id)
        {
            IQueryable<TIPO_CONDOMINIO> query = Db.TIPO_CONDOMINIO;
            query = query.Where(p => p.TICO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CONDOMINIO> GetAllItensAdm()
        {
            IQueryable<TIPO_CONDOMINIO> query = Db.TIPO_CONDOMINIO;
            return query.ToList();
        }

        public List<TIPO_CONDOMINIO> GetAllItens()
        {
            IQueryable<TIPO_CONDOMINIO> query = Db.TIPO_CONDOMINIO.Where(p => p.TICO_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 