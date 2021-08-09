using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMudancaAnexoRepository : IRepositoryBase<SOLICITACAO_MUDANCA_ANEXO>
    {
        List<SOLICITACAO_MUDANCA_ANEXO> GetAllItens(Int32 idAss);
        SOLICITACAO_MUDANCA_ANEXO GetItemById(Int32 id);
    }
}
