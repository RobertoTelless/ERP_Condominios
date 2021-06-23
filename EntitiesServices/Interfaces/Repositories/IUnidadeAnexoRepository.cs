using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUnidadeAnexoRepository : IRepositoryBase<UNIDADE_ANEXO>
    {
        List<UNIDADE_ANEXO> GetAllItens();
        UNIDADE_ANEXO GetItemById(Int32 id);
    }
}
