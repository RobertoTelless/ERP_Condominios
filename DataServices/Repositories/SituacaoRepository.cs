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
    public class SituacaoRepository : RepositoryBase<SITUACAO>, ISituacaoRepository
    {
        public SITUACAO GetItemById(Int32 id)
        {
            IQueryable<SITUACAO> query = Db.SITUACAO;
            query = query.Where(p => p.SITU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SITUACAO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<SITUACAO> query = Db.SITUACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SITUACAO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<SITUACAO> query = Db.SITUACAO.Where(p => p.SITU_NM_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 