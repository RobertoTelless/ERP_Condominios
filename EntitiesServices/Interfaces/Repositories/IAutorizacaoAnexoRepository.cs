using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAutorizacaoAnexoRepository : IRepositoryBase<AUTORIZACAO_ACESSO_ANEXO>
    {
        List<AUTORIZACAO_ACESSO_ANEXO> GetAllItens();
        AUTORIZACAO_ACESSO_ANEXO GetItemById(Int32 id);
    }
}
