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
    public class AssinanteAnexoRepository : RepositoryBase<ASSINANTE_ANEXO>, IAssinanteAnexoRepository
    {
        public List<ASSINANTE_ANEXO> GetAllItens()
        {
            return Db.ASSINANTE_ANEXO.ToList();
        }

        public ASSINANTE_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_ANEXO> query = Db.ASSINANTE_ANEXO.Where(p => p.ASAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 