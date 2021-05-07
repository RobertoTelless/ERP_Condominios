using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFuncionarioAnexoRepository : IRepositoryBase<FUNCIONARIO_ANEXO>
    {
        List<FUNCIONARIO_ANEXO> GetAllItens();
        FUNCIONARIO_ANEXO GetItemById(Int32 id);
    }
}
