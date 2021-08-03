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
    public class VeiculoRepository : RepositoryBase<VEICULO>, IVeiculoRepository
    {
        public VEICULO CheckExist(VEICULO tarefa, Int32 idAss)
        {
            IQueryable<VEICULO> query = Db.VEICULO;
            query = query.Where(p => p.VEIC_NM_PLACA == tarefa.VEIC_NM_PLACA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public VEICULO GetItemById(Int32 id)
        {
            IQueryable<VEICULO> query = Db.VEICULO;
            query = query.Where(p => p.VEIC_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.VAGA);
            query = query.Include(p => p.VEICULO_ANEXO);
            return query.FirstOrDefault();
        }

        public List<VEICULO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VEICULO> query = Db.VEICULO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VEICULO> GetAllItens(Int32 idAss)
        {
            IQueryable<VEICULO> query = Db.VEICULO.Where(p => p.VEIC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VEICULO> GetByUnidade(Int32 idUnid)
        {
            IQueryable<VEICULO> query = Db.VEICULO.Where(p => p.VEIC_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, Int32? vaga, Int32 idAss)
        {
            List<VEICULO> lista = new List<VEICULO>();
            IQueryable<VEICULO> query = Db.VEICULO;
            if (!String.IsNullOrEmpty(placa))
            {
                query = query.Where(p => p.VEIC_NM_PLACA == placa);
            }
            if (!String.IsNullOrEmpty(marca))
            {
                query = query.Where(p => p.VEIC_NM_MARCA.Contains(marca));
            }
            if (unid > 0)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (idTipo > 0)
            {
                query = query.Where(p => p.TIVE_CD_ID == idTipo);
            }
            if (vaga > 0)
            {
                query = query.Where(p => p.VAGA_CD_ID == vaga);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.VEIC_NM_PLACA);
                lista = query.ToList<VEICULO>();
            }
            return lista;
        }
    }
}
 