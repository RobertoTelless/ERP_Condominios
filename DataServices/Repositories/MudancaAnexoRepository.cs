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
    public class MudancaAnexoRepository : RepositoryBase<SOLICITACAO_MUDANCA_ANEXO>, IMudancaAnexoRepository
    {
        public List<SOLICITACAO_MUDANCA_ANEXO> GetAllItens(Int32 idAss)
        {
            return Db.SOLICITACAO_MUDANCA_ANEXO.ToList();
        }

        public SOLICITACAO_MUDANCA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<SOLICITACAO_MUDANCA_ANEXO> query = Db.SOLICITACAO_MUDANCA_ANEXO.Where(p => p.SOAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 