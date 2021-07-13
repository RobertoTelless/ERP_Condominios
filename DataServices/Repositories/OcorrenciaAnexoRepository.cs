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
    public class OcorrenciaAnexoRepository : RepositoryBase<OCORRENCIA_ANEXO>, IOcorrenciaAnexoRepository
    {
        public List<OCORRENCIA_ANEXO> GetAllItens()
        {
            return Db.OCORRENCIA_ANEXO.ToList();
        }

        public OCORRENCIA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<OCORRENCIA_ANEXO> query = Db.OCORRENCIA_ANEXO.Where(p => p.OCAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 