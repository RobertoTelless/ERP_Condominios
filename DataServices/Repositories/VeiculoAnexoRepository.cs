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
    public class VeiculoAnexoRepository : RepositoryBase<VEICULO_ANEXO>, IVeiculoAnexoRepository
    {
        public List<VEICULO_ANEXO> GetAllItens()
        {
            return Db.VEICULO_ANEXO.ToList();
        }

        public VEICULO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<VEICULO_ANEXO> query = Db.VEICULO_ANEXO.Where(p => p.VEAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 