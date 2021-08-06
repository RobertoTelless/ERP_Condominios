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
    public class CorpoDiretivoRepository : RepositoryBase<CORPO_DIRETIVO>, ICorpoDiretivoRepository
    {
        public CORPO_DIRETIVO CheckExist(CORPO_DIRETIVO conta, Int32 idAss)
        {
            IQueryable<CORPO_DIRETIVO> query = Db.CORPO_DIRETIVO;
            query = query.Where(p => p.USUA_CD_ID == conta.USUA_CD_ID);
            query = query.Where(p => p.CODI_DT_FINAL == null);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CORPO_DIRETIVO GetItemById(Int32 id)
        {
            IQueryable<CORPO_DIRETIVO> query = Db.CORPO_DIRETIVO;
            query = query.Where(p => p.CODI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CORPO_DIRETIVO> query = Db.CORPO_DIRETIVO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CORPO_DIRETIVO> GetAllItens(Int32 idAss)
        {
            IQueryable<CORPO_DIRETIVO> query = Db.CORPO_DIRETIVO.Where(p => p.CODI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 