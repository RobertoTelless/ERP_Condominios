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
    public class ControleVeiculoAcompanhamentoRepository : RepositoryBase<CONTROLE_VEICULO_ACOMPANHAMENTO>, IControleVeiculoAcompanhamentoRepository
    {
        public List<CONTROLE_VEICULO_ACOMPANHAMENTO> GetAllItens()
        {
            return Db.CONTROLE_VEICULO_ACOMPANHAMENTO.ToList();
        }

        public CONTROLE_VEICULO_ACOMPANHAMENTO GetItemById(Int32 id)
        {
            IQueryable<CONTROLE_VEICULO_ACOMPANHAMENTO> query = Db.CONTROLE_VEICULO_ACOMPANHAMENTO.Where(p => p.CVAC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 