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
    public class EntradaSaidaRepository : RepositoryBase<ENTRADA_SAIDA>, IEntradaSaidaRepository
    {
        public ENTRADA_SAIDA GetItemById(Int32 id)
        {
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA;
            query = query.Where(p => p.ENSA_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.ENTRADA_SAIDA_COMENTARIO);
            return query.FirstOrDefault();
        }

        public List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ENTRADA_SAIDA> GetAllItens(Int32 idAss)
        {
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA.Where(p => p.ENSA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid)
        {
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA.Where(p => p.ENSA_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<ENTRADA_SAIDA> GetByData(DateTime data, Int32 idAss)
        {
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA.Where(p => p.ENSA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.ENSA_DT_ENTRADA) == DbFunctions.TruncateTime(data));
            return query.ToList();
        }

        public List<ENTRADA_SAIDA> ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada,  DateTime? dataSaida, Int32? status, Int32 idAss)
        {
            List<ENTRADA_SAIDA> lista = new List<ENTRADA_SAIDA>();
            IQueryable<ENTRADA_SAIDA> query = Db.ENTRADA_SAIDA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ENSA_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(documento))
            {
                query = query.Where(p => p.ENSA_NR_DOCUMENTO.Contains(documento));
            }
            if (unid > 0)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (autorizacao > 0)
            {
                query = query.Where(p => p.AUAC_CD_ID == autorizacao);
            }
            if (status > 0)
            {
                query = query.Where(p => p.ENSA_IN_STATUS == status);
            }
            if (dataEntrada != null & dataSaida == null)
            {
                query = query.Where(p => p.ENSA_DT_ENTRADA == dataEntrada);
            }
            else if (dataEntrada == null & dataSaida != null)
            {
                query = query.Where(p => p.ENSA_DT_SAIDA == dataSaida);
            }
            else if (dataEntrada != null & dataSaida != null)
            {
                query = query.Where(p => p.ENSA_DT_SAIDA <= dataSaida & p.ENSA_DT_ENTRADA >= dataEntrada);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.ENSA_DT_ENTRADA);
                lista = query.ToList<ENTRADA_SAIDA>();
            }
            return lista;
        }
    }
}
 