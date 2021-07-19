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
    public class EquipamentoManutencaoRepository : RepositoryBase<EQUIPAMENTO_MANUTENCAO>, IEquipamentoManutencaoRepository
    {
        public List<EQUIPAMENTO_MANUTENCAO> GetAllItens(Int32 idAss)
        {
            return Db.EQUIPAMENTO_MANUTENCAO.ToList();
        }

        public EQUIPAMENTO_MANUTENCAO GetItemById(Int32 id)
        {
            IQueryable<EQUIPAMENTO_MANUTENCAO> query = Db.EQUIPAMENTO_MANUTENCAO.Where(p => p.EQMA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 