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
    public class ReservaRepository : RepositoryBase<RESERVA>, IReservaRepository
    {
        public RESERVA CheckExist(RESERVA tarefa, Int32 idAss)
        {
            IQueryable<RESERVA> query = Db.RESERVA;
            query = query.Where(p => p.RESE_DT_EVENTO == tarefa.RESE_DT_EVENTO);
            query = query.Where(p => p.UNID_CD_ID == tarefa.UNID_CD_ID);
            query = query.Where(p => p.RESE_NM_NOME == tarefa.RESE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public RESERVA GetItemById(Int32 id)
        {
            IQueryable<RESERVA> query = Db.RESERVA;
            query = query.Where(p => p.RESE_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<RESERVA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<RESERVA> query = Db.RESERVA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<RESERVA> GetAllItens(Int32 idAss)
        {
            IQueryable<RESERVA> query = Db.RESERVA.Where(p => p.RESE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<RESERVA> GetByUnidade(Int32 idUnid)
        {
            IQueryable<RESERVA> query = Db.RESERVA.Where(p => p.RESE_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<RESERVA> ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32 idAss)
        {
            List<RESERVA> lista = new List<RESERVA>();
            IQueryable<RESERVA> query = Db.RESERVA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.RESE_NM_NOME.Contains(nome));
            }
            if (data != null)
            {
                query = query.Where(p => p.RESE_DT_EVENTO == data);
            }
            if (unidade != null)
            {
                query = query.Where(p => p.UNID_CD_ID == unidade);
            }
            if (finalidade != null)
            {
                query = query.Where(p => p.FIRE_CD_ID == finalidade);
            }
            if (ambiente != null)
            {
                query = query.Where(p => p.AMBI_CD_ID == ambiente);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.RESE_DT_EVENTO);
                lista = query.ToList<RESERVA>();
            }
            return lista;
        }
    }
}
 