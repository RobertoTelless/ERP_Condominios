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
    public class MudancaRepository : RepositoryBase<SOLICITACAO_MUDANCA>, IMudancaRepository
    {
        public SOLICITACAO_MUDANCA CheckExist(SOLICITACAO_MUDANCA tarefa, Int32 idAss)
        {
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA;
            query = query.Where(p => p.SOMU_DT_MUDANCA == tarefa.SOMU_DT_MUDANCA);
            query = query.Where(p => p.UNID_CD_ID == tarefa.UNID_CD_ID);
            query = query.Where(p => p.SOMU_IN_ENTRADA_SAIDA == tarefa.SOMU_IN_ENTRADA_SAIDA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public SOLICITACAO_MUDANCA GetItemById(Int32 id)
        {
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA;
            query = query.Where(p => p.SOMU_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<SOLICITACAO_MUDANCA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SOLICITACAO_MUDANCA> GetAllItens(Int32 idAss)
        {
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA.Where(p => p.SOMU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SOLICITACAO_MUDANCA> GetByUnidade(Int32 idUnid)
        {
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA.Where(p => p.SOMU_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<SOLICITACAO_MUDANCA> ExecuteFilter(DateTime? data, Int32? entrada, Int32? status, Int32 idAss)
        {
            List<SOLICITACAO_MUDANCA> lista = new List<SOLICITACAO_MUDANCA>();
            IQueryable<SOLICITACAO_MUDANCA> query = Db.SOLICITACAO_MUDANCA;
            if (data != null)
            {
                query = query.Where(p => p.SOMU_DT_MUDANCA == data);
            }
            if (entrada > 0)
            {
                query = query.Where(p => p.SOMU_IN_ENTRADA_SAIDA == entrada);
            }
            if (status > 0)
            {
                query = query.Where(p => p.SOMU_IN_STATUS == status);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.SOMU_DT_MUDANCA);
                lista = query.ToList<SOLICITACAO_MUDANCA>();
            }
            return lista;
        }
    }
}
 