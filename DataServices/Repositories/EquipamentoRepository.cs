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
    public class EquipamentoRepository : RepositoryBase<EQUIPAMENTO>, IEquipamentoRepository
    {
        public EQUIPAMENTO CheckExist(EQUIPAMENTO conta)
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO;
            query = query.Where(p => p.EQUI_NR_NUMERO == conta.EQUI_NR_NUMERO);
            return query.FirstOrDefault();
        }

        public EQUIPAMENTO GetByNumero(String numero)
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO.Where(p => p.EQUI_IN_ATIVO == 1);
            query = query.Where(p => p.EQUI_NR_NUMERO == numero);
            query = query.Include(p => p.PERIODICIDADE);
            query = query.Include(p => p.EQUIPAMENTO_ANEXO);
            query = query.Include(p => p.EQUIPAMENTO_MANUTENCAO);
            return query.FirstOrDefault();
        }

        public EQUIPAMENTO GetItemById(Int32 id)
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO;
            query = query.Where(p => p.EQUI_CD_ID == id);
            query = query.Include(p => p.PERIODICIDADE);
            query = query.Include(p => p.EQUIPAMENTO_ANEXO);
            query = query.Include(p => p.EQUIPAMENTO_MANUTENCAO);
            return query.FirstOrDefault();
        }

        public List<EQUIPAMENTO> GetAllItens()
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO.Where(p => p.EQUI_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<EQUIPAMENTO> GetAllItensAdm()
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO;
            return query.ToList();
        }

        public Int32 CalcularManutencaoVencida()
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO.Where(p => p.EQUI_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.AddDays(p.EQUI_DT_MANUTENCAO.Value, p.PERIODICIDADE.PERI_NR_DIAS) < DateTime.Today);
            return query.ToList().Count;
        }

        public Int32 CalcularDepreciados()
        {
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO.Where(p => p.EQUI_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.AddDays(p.EQUI_DT_COMPRA.Value, (p.EQUI_NR_VIDA_UTIL.Value * 30)) < DateTime.Today);
            return query.ToList().Count;
        }

        public List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao)
        {
            List<EQUIPAMENTO> lista = new List<EQUIPAMENTO>();
            IQueryable<EQUIPAMENTO> query = Db.EQUIPAMENTO;
            if (catId != null)
            {
                query = query.Where(p => p.CATEGORIA_EQUIPAMENTO.CAEQ_CD_ID == catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.EQUI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.EQUI_NR_NUMERO == numero);
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.EQUI_NR_NUMERO);
                lista = query.ToList<EQUIPAMENTO>();

                if (depreciado == 1)
                {
                    lista = lista.Where(p => p.EQUI_DT_COMPRA.Value.AddDays(p.EQUI_NR_VIDA_UTIL.Value * 30).Date < DateTime.Today.Date & p.EQUI_IN_ATIVO == 1).ToList<EQUIPAMENTO>();
                }
                if (manutencao == 1)
                {
                    lista = lista.Where(p => DbFunctions.AddDays(p.EQUI_DT_MANUTENCAO.Value, p.PERIODICIDADE.PERI_NR_DIAS) < DateTime.Today).ToList<EQUIPAMENTO>();
                }
            }
            return lista;
        }
    }
}
 