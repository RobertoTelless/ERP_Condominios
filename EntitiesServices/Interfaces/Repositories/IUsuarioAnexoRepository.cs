using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUsuarioAnexoRepository : IRepositoryBase<USUARIO_ANEXO>
    {
        List<USUARIO_ANEXO> GetAllItens();
        USUARIO_ANEXO GetItemById(Int32 id);
    }
}
