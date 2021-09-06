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
    public class ControleVeiculoRepository : RepositoryBase<CONTROLE_VEICULO>, IControleVeiculoRepository
    {
        public CONTROLE_VEICULO GetItemById(Int32 id)
        {
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO;
            query = query.Where(p => p.COVE_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CONTROLE_VEICULO_ACOMPANHAMENTO);
            return query.FirstOrDefault();
        }

        public List<CONTROLE_VEICULO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTROLE_VEICULO> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO.Where(p => p.COVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTROLE_VEICULO> GetByUnidade(Int32 idUnid)
        {
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO.Where(p => p.COVE_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<CONTROLE_VEICULO> GetByData(DateTime data, Int32 idAss)
        {
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO.Where(p => p.COVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.COVE_DT_ENTRADA) == DbFunctions.TruncateTime(data));
            return query.ToList();
        }

        public List<CONTROLE_VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, DateTime? dataEntrada, DateTime? dataSaida, Int32 idAss)
        {
            List<CONTROLE_VEICULO> lista = new List<CONTROLE_VEICULO>();
            IQueryable<CONTROLE_VEICULO> query = Db.CONTROLE_VEICULO;
            if (!String.IsNullOrEmpty(placa))
            {
                query = query.Where(p => p.COVE_NM_PLACA == placa);
            }
            if (!String.IsNullOrEmpty(marca))
            {
                query = query.Where(p => p.COVE_NM_MARCA.Contains(marca));
            }
            if (unid != null)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (idTipo > 0)
            {
                query = query.Where(p => p.TIVE_CD_ID == idTipo);
            }
            if (dataEntrada != DateTime.MinValue & dataSaida == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COVE_DT_ENTRADA) == DbFunctions.TruncateTime(dataEntrada));
            }
            else if (dataEntrada == DateTime.MinValue & dataSaida != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COVE_DT_SAIDA) == DbFunctions.TruncateTime(dataSaida));
            }
            else if (dataEntrada != DateTime.MinValue & dataSaida != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COVE_DT_SAIDA) <= DbFunctions.TruncateTime(dataSaida) & DbFunctions.TruncateTime(p.COVE_DT_ENTRADA) >= DbFunctions.TruncateTime(dataEntrada));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.COVE_DT_ENTRADA);
                lista = query.ToList<CONTROLE_VEICULO>();
            }
            return lista;
        }
    }
}
 