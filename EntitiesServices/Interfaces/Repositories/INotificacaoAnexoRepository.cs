using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INotificacaoAnexoRepository : IRepositoryBase<NOTIFICACAO_ANEXO>
    {
        List<NOTIFICACAO_ANEXO> GetAllItens();
        NOTIFICACAO_ANEXO GetItemById(Int32 id);
    }
}
