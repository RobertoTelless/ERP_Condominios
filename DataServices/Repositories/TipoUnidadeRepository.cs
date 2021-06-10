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
    public class TipoUnidadeRepository : RepositoryBase<TIPO_UNIDADE>, ITipoUnidadeRepository
    {
        public TIPO_UNIDADE GetItemById(Int32 id)
        {
            IQueryable<TIPO_UNIDADE> query = Db.TIPO_UNIDADE;
            query = query.Where(p => p.TIUN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_UNIDADE> query = Db.TIPO_UNIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_UNIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_UNIDADE> query = Db.TIPO_UNIDADE.Where(p => p.TIUN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 