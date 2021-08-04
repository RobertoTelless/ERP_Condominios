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
using CrossCutting;

namespace DataServices.Repositories
{
    public class AmbienteRepository : RepositoryBase<AMBIENTE>, IAmbienteRepository
    {
        public AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss)
        {
            IQueryable<AMBIENTE> query = Db.AMBIENTE;
            query = query.Where(p => p.AMBI_NM_AMBIENTE == conta.AMBI_NM_AMBIENTE);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public AMBIENTE GetItemById(Int32 id)
        {
            IQueryable<AMBIENTE> query = Db.AMBIENTE;
            query = query.Where(p => p.AMBI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<AMBIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<AMBIENTE> query = Db.AMBIENTE.Where(p => p.AMBI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AMBIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<AMBIENTE> query = Db.AMBIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AMBIENTE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss)
        {
            List<AMBIENTE> lista = new List<AMBIENTE>();
            IQueryable<AMBIENTE> query = Db.AMBIENTE;
            if (tipo > 0)
            {
                query = query.Where(p => p.TIAM_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.AMBI_NM_AMBIENTE.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.AMBI_NM_AMBIENTE);
                lista = query.ToList<AMBIENTE>();
            }
            return lista;
        }

    }
}
 