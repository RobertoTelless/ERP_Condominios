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
    public class UnidadeRepository : RepositoryBase<UNIDADE>, IUnidadeRepository
    {
        public UNIDADE CheckExist(UNIDADE tarefa, Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.UNID_NR_NUMERO == tarefa.UNID_NR_NUMERO);
            query = query.Where(p => p.TORR_CD_ID == tarefa.TORR_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public UNIDADE GetItemById(Int32 id)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.UNID_CD_ID == id);
            query = query.Include(p => p.ENCOMENDA);
            query = query.Include(p => p.ENTRADA_SAIDA);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.VEICULO);
            query = query.Include(p => p.RESERVA);
            query = query.Include(p => p.SOLICITACAO_MUDANCA);
            query = query.Include(p => p.LISTA_CONVIDADO);
            query = query.Include(p => p.OCORRENCIA);
            return query.FirstOrDefault();
        }

        public List<UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<UNIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE.Where(p => p.UNID_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<UNIDADE> ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss)
        {
            List<UNIDADE> lista = new List<UNIDADE>();
            IQueryable<UNIDADE> query = Db.UNIDADE;
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.UNID_NR_NUMERO == numero);
            }
            if (torre != 0)
            {
                query = query.Where(p => p.TORR_CD_ID == torre);
            }
            if (idTipo != null)
            {
                query = query.Where(p => p.TIUN_CD_ID == idTipo);
            }
            if (alugada != null)
            {
                query = query.Where(p => p.UNID_IN_ALUGADA == alugada);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.UNID_NM_EXIBE);
                lista = query.ToList<UNIDADE>();
            }
            return lista;
        }
    }
}
 