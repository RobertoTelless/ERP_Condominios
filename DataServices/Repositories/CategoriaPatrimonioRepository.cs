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
    public class CategoriaPatrimonioRepository : RepositoryBase<CATEGORIA_PATRIMONIO>, ICategoriaPatrimonioRepository
    {
        public CATEGORIA_PATRIMONIO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_PATRIMONIO> query = Db.CATEGORIA_PATRIMONIO;
            query = query.Where(p => p.CAPA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_PATRIMONIO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_PATRIMONIO> query = Db.CATEGORIA_PATRIMONIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_PATRIMONIO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_PATRIMONIO> query = Db.CATEGORIA_PATRIMONIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 