using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMudancaRepository : IRepositoryBase<SOLICITACAO_MUDANCA>
    {
        SOLICITACAO_MUDANCA CheckExist(SOLICITACAO_MUDANCA item, Int32 idAss);
        SOLICITACAO_MUDANCA GetItemById(Int32 id);
        List<SOLICITACAO_MUDANCA> GetAllItens(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetAllItensAdm(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetByUnidade(Int32 idUnid);
        List<SOLICITACAO_MUDANCA> ExecuteFilter(DateTime? data, Int32? entrada, Int32? status, Int32 idAss);
    }
}
