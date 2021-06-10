using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinanteAnexoRepository : IRepositoryBase<ASSINANTE_ANEXO>
    {
        List<ASSINANTE_ANEXO> GetAllItens();
        ASSINANTE_ANEXO GetItemById(Int32 id);
    }
}
