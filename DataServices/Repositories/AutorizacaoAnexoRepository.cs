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
    public class AutorizacaoAnexoRepository : RepositoryBase<AUTORIZACAO_ACESSO_ANEXO>, IAutorizacaoAnexoRepository
    {
        public List<AUTORIZACAO_ACESSO_ANEXO> GetAllItens()
        {
            return Db.AUTORIZACAO_ACESSO_ANEXO.ToList();
        }

        public AUTORIZACAO_ACESSO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<AUTORIZACAO_ACESSO_ANEXO> query = Db.AUTORIZACAO_ACESSO_ANEXO.Where(p => p.AAAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 