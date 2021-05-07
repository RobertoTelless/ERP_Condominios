using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPatrimonioAnexoRepository : IRepositoryBase<PATRIMONIO_ANEXO>
    {
        List<PATRIMONIO_ANEXO> GetAllItens();
        PATRIMONIO_ANEXO GetItemById(Int32 id);
    }
}
