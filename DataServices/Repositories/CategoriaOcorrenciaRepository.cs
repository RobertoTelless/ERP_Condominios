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
    public class CategoriaOcorrenciaRepository : RepositoryBase<CATEGORIA_OCORRENCIA>, ICategoriaOcorrenciaRepository
    {
        public CATEGORIA_OCORRENCIA GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_OCORRENCIA> query = Db.CATEGORIA_OCORRENCIA;
            query = query.Where(p => p.CAOC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_OCORRENCIA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_OCORRENCIA> query = Db.CATEGORIA_OCORRENCIA;
            return query.ToList();
        }

        public List<CATEGORIA_OCORRENCIA> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_OCORRENCIA> query = Db.CATEGORIA_OCORRENCIA.Where(p => p.CAOC_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 