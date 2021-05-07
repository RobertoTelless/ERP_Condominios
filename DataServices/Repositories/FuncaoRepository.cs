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
    public class FuncaoRepository : RepositoryBase<FUNCAO>, IFuncaoRepository
    {
        public FUNCAO GetItemById(Int32 id)
        {
            IQueryable<FUNCAO> query = Db.FUNCAO;
            query = query.Where(p => p.FNCA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FUNCAO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FUNCAO> query = Db.FUNCAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<FUNCAO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FUNCAO> query = Db.FUNCAO.Where(p => p.FNCA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 