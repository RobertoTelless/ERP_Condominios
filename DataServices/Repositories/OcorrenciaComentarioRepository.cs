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
    public class OcorrenciaComentarioRepository : RepositoryBase<OCORRENCIA_COMENTARIO>, IOcorrenciaComentarioRepository
    {
        public List<OCORRENCIA_COMENTARIO> GetAllItens()
        {
            return Db.OCORRENCIA_COMENTARIO.ToList();
        }

        public OCORRENCIA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<OCORRENCIA_COMENTARIO> query = Db.OCORRENCIA_COMENTARIO.Where(p => p.OCCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 