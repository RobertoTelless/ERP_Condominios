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
    public class EquipamentoAnexoRepository : RepositoryBase<EQUIPAMENTO_ANEXO>, IEquipamentoAnexoRepository
    {
        public List<EQUIPAMENTO_ANEXO> GetAllItens(Int32 idAss)
        {
            return Db.EQUIPAMENTO_ANEXO.ToList();
        }

        public EQUIPAMENTO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<EQUIPAMENTO_ANEXO> query = Db.EQUIPAMENTO_ANEXO.Where(p => p.EQAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 