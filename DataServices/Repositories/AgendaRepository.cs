using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class AgendaRepository : RepositoryBase<AGENDA>, IAgendaRepository
    {
        public List<AGENDA> GetByDate(DateTime data, Int32 idAss)
        {
            IQueryable<AGENDA> query = Db.AGENDA.Where(p => p.AGEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.AGEN_DT_DATA == data);
            query = query.Include(p => p.AGENDA_ANEXO);
            return query.ToList();
        }

        public List<AGENDA> GetByUser(Int32 id, Int32 idAss)
        {
            IQueryable<AGENDA> query = Db.AGENDA.Where(p => p.AGEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.AGENDA_ANEXO);
            return query.ToList();
        }

        public AGENDA GetItemById(Int32 id)
        {
            IQueryable<AGENDA> query = Db.AGENDA;
            query = query.Where(p => p.AGEN_CD_ID == id);
            query = query.Include(p => p.AGENDA_ANEXO);
            return query.FirstOrDefault();
        }

        public List<AGENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<AGENDA> query = Db.AGENDA.Where(p => p.AGEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AGENDA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<AGENDA> query = Db.AGENDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser)
        {
            List<AGENDA> lista = new List<AGENDA>();
            IQueryable<AGENDA> query = Db.AGENDA.Where(x => x.USUA_CD_ID == idUser);
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.AGEN_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.AGEN_DS_DESCRICAO.Contains(descricao));
            }
            if (cat > 0 & cat != null)
            {
                query = query.Where(p => p.CAAG_CD_ID == cat);
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderByDescending(a => a.AGEN_DT_DATA).ThenByDescending(b => b.AGEN_HR_HORA);
                lista = query.ToList<AGENDA>();
                if (data != DateTime.MinValue)
                {
                    lista = lista.Where(p => p.AGEN_DT_DATA == data).ToList<AGENDA>();
                }
            }
            return lista;
        }
    }
}
 