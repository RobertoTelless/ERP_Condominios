using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IListaConvidadoAnexoRepository : IRepositoryBase<LISTA_CONVIDADO_ANEXO>
    {
        List<LISTA_CONVIDADO_ANEXO> GetAllItens();
        LISTA_CONVIDADO_ANEXO GetItemById(Int32 id);
    }
}
