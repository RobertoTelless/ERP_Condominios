using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaNotificacaoRepository : IRepositoryBase<CATEGORIA_NOTIFICACAO>
    {
        List<CATEGORIA_NOTIFICACAO> GetAllItens();
        CATEGORIA_NOTIFICACAO GetItemById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllItensAdm();
    }
}
