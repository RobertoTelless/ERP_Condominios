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
    public class ListaConvidadoRepository : RepositoryBase<LISTA_CONVIDADO>, IListaConvidadoRepository
    {
        public LISTA_CONVIDADO CheckExist(LISTA_CONVIDADO tarefa, Int32 idAss)
        {
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO;
            query = query.Where(p => p.LICO_DT_EVENTO == tarefa.LICO_DT_EVENTO);
            query = query.Where(p => p.UNID_CD_ID == tarefa.UNID_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public LISTA_CONVIDADO GetItemById(Int32 id)
        {
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO;
            query = query.Where(p => p.LICO_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.CONVIDADO);
            return query.FirstOrDefault();
        }

        public List<LISTA_CONVIDADO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONVIDADO);
            return query.ToList();
        }

        public List<LISTA_CONVIDADO> GetAllItens(Int32 idAss)
        {
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO.Where(p => p.LICO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONVIDADO);
            return query.ToList();
        }

        public List<LISTA_CONVIDADO> GetByUnidade(Int32 idUnid)
        {
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO.Where(p => p.LICO_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            query = query.Include(p => p.CONVIDADO);
            return query.ToList();
        }

        public List<LISTA_CONVIDADO> ExecuteFilter(String nome, DateTime? data, Int32? unid, Int32? reserva, Int32 idAss)
        {
            List<LISTA_CONVIDADO> lista = new List<LISTA_CONVIDADO>();
            IQueryable<LISTA_CONVIDADO> query = Db.LISTA_CONVIDADO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.LICO_NM_LISTA.Contains(nome));
            }
            if (data != null)
            {
                query = query.Where(p => p.LICO_DT_EVENTO == data);
            }
            if (unid != null)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (reserva != null)
            {
                query = query.Where(p => p.RESE_CD_ID == reserva);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.LICO_DT_EVENTO);
                query = query.Include(p => p.CONVIDADO);
                lista = query.ToList<LISTA_CONVIDADO>();
            }
            return lista;
        }
    }
}
 