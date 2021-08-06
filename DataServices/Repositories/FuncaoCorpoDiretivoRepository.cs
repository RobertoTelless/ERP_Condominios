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
    public class FuncaoCorpoDiretivoRepository : RepositoryBase<FUNCAO_CORPO_DIRETIVO>, IFuncaoCorpoDiretivoRepository
    {
        public FUNCAO_CORPO_DIRETIVO GetItemById(Int32 id)
        {
            IQueryable<FUNCAO_CORPO_DIRETIVO> query = Db.FUNCAO_CORPO_DIRETIVO;
            query = query.Where(p => p.FUCO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FUNCAO_CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<FUNCAO_CORPO_DIRETIVO> query = Db.FUNCAO_CORPO_DIRETIVO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<FUNCAO_CORPO_DIRETIVO> GetAllItens(Int32 idAss)
        {
            IQueryable<FUNCAO_CORPO_DIRETIVO> query = Db.FUNCAO_CORPO_DIRETIVO.Where(p => p.FUCO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 